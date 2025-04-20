using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_WebUp.Data;
using Movie_WebUp.Model;

namespace Movie_WebUp.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly Movie_WebUp.Data.Movie_WebUpContext _context;

        public IndexModel(Movie_WebUp.Data.Movie_WebUpContext context)
        {
            _context = context;
        }

        public List<Movie> MoviesList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? ListGenres { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MovieGenreName { get; set; }

        [BindProperty]
        public int? MovieId { get; set; }

        public async Task OnGetAsync(int? id)
        {
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;

            var movies = from m in _context.Movie
                         select m;

            if (!string.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(s => s.Title.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(MovieGenreName))
            {
                movies = movies.Where(x => x.Genre == MovieGenreName);
            }

            ListGenres = new SelectList(await genreQuery.Distinct().ToListAsync());
            MoviesList = await movies.OrderByDescending(m => m.CreatedAt).ToListAsync();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnGetFilterMoviesAsync(string? genre, string? search)
        {
            var filteredMovies = _context.Movie.AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                filteredMovies = filteredMovies.Where(m => m.Genre == genre);
            }

            if (!string.IsNullOrEmpty(search))
            {
                filteredMovies = filteredMovies.Where(m => m.Title.Contains(search));
            }

            var moviesList = await filteredMovies.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return Partial("_MovieTablePartial", moviesList);
        }

        public async Task<IActionResult> OnGetSearchAsync(string term, string genre)
        {
            if (string.IsNullOrEmpty(term))
            {
                return new JsonResult(new List<string>());
            }

            var movieTitlesQuery = _context.Movie.AsQueryable();

            if (!string.IsNullOrEmpty(genre))
            {
                movieTitlesQuery = movieTitlesQuery.Where(m => m.Genre == genre);
            }

            var movieTitles = await movieTitlesQuery
                .Where(m => m.Title.Contains(term))
                .Select(m => m.Title)
                .ToListAsync();

            return new JsonResult(movieTitles);
        }

    }
}