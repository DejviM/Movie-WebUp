using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_WebUp.Data;
using Movie_WebUp.Model;
using System.Globalization;
using System.Text.Json;

namespace Movie_WebUp.Pages.Movies
{
    public class CategoryModel : PageModel
    {
        private readonly Movie_WebUpContext _context;

        public CategoryModel(Movie_WebUpContext context)
        {
            _context = context;
        }

        public List<string> Genres { get; set; } = new();
        public List<Movie> MoviesList { get; set; } = new();
        public string SelectedGenre { get; set; }

        public async Task OnGetAsync(string? genre)
        {
            Genres = await _context.Movie
                .Select(m => m.Genre)
                .Distinct()
                .ToListAsync();

            if (!string.IsNullOrEmpty(genre))
            {
                SelectedGenre = genre;
                MoviesList = await _context.Movie
                    .Where(m => m.Genre == genre)
                    .Include(m => m.MovieRatings)
                    .Select(m => new Movie
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Genre = m.Genre,
                        Photo = m.Photo,
                        PhotoContentType = m.PhotoContentType,
                        Price = m.Price,
                        Description = m.Description,
                        CreatedAt = m.CreatedAt,
                        AverageRating = m.MovieRatings.Any() ? m.MovieRatings.Average(r => r.Rating) : 0
                    })
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
        }

        public async Task<JsonResult> OnGetGenreViewsAsync(string genre, int year)
        {
            var movieIds = await _context.Movie
                .Where(m => m.Genre == genre)
                .Select(m => m.Id)
                .ToListAsync();

            if (!movieIds.Any())
            {
                return new JsonResult(new List<object>()); // empty json response
            }

            var genreViews = await _context.MovieViewRecords
                .Where(v => movieIds.Contains(v.MovieId) && v.ViewDate.Year == year)
                .ToListAsync();

            var monthlyViews = Enumerable.Range(1, 12)  // Ensure all months are represented
                .Select(month => new
                {
                    Month = month, // Keep it as a number
                    Views = genreViews.Where(v => v.ViewDate.Month == month).Sum(v => v.ViewCount)
                })
                .ToList();

            return new JsonResult(monthlyViews);
        }

    }
}