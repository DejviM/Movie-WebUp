using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_WebUp.Model;
using Newtonsoft.Json;

namespace Movie_WebUp.Pages;

public class IndexModel : PageModel
{
    private readonly Movie_WebUp.Data.Movie_WebUpContext _context;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(Movie_WebUp.Data.Movie_WebUpContext context, ILogger<IndexModel> logger)
    {
        _context = context;
    }
    public Movie LatestMovie { get; set; } // Latest movie
    public List<Movie> MostViewedMovies { get; set; } // 3 most viewed movies for the hero carousel
    public List<Movie> Latest12Movies { get; set; } // 12 latest movies for the second carousel
    public List<Movie> AllMovies { get; set; } // All movies for the grid


    public async Task OnGetAsync()
    {
        // Get the latest movie
        LatestMovie = await _context.Movie
            .OrderByDescending(f => f.CreatedAt)
            .FirstOrDefaultAsync();

        // Get the 3 most viewed movies for the hero carousel
        MostViewedMovies = await _context.Movie
            .OrderByDescending(m => m.Views) // Order by Views in descending order
            .Take(3) // Take the top 3 movies
            .ToListAsync();


        // Get the 12 latest movies for the second carousel
        Latest12Movies = await _context.Movie
            .OrderByDescending(f => f.CreatedAt)
            .Take(12)
            .ToListAsync();

        // Get all movies for the grid include the rate 
        AllMovies = await _context.Movie
               .Include(m => m.MovieRatings) // Include ratings
               .Select(m => new Movie
               {
                   Id = m.Id,
                   Title = m.Title,
                   Genre = m.Genre,
                   Photo = m.Photo,
                   PhotoContentType = m.PhotoContentType, // Include PhotoContentType
                   Price = m.Price, // Include Price
                   Description = m.Description, // Include Description
                   CreatedAt = m.CreatedAt,
                   AverageRating = m.MovieRatings.Any() ? m.MovieRatings.Average(r => r.Rating) : 0 // Calculate average rating
               })
               .OrderByDescending(f => f.CreatedAt)
               .ToListAsync();

    }

    public async Task<IActionResult> OnGetSearchAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return new JsonResult(new List<string>()); // Return empty list if search term is empty
        }

        var movieTitles = await _context.Movie
            .Where(m => m.Title.Contains(term)) // Case-insensitive search
            .OrderBy(m => m.Title)
            .Select(m => m.Title)
            .Take(10) // Limit results
            .ToListAsync();

        return new JsonResult(movieTitles);
    }
}
