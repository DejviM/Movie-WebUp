using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Movie_WebUp.Data;
using Movie_WebUp.Model;

namespace Movie_WebUp.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly Movie_WebUp.Data.Movie_WebUpContext _context;

        public DetailsModel(Movie_WebUp.Data.Movie_WebUpContext context)
        {
            _context = context;
        }

        public Movie MovieObj { get; set; } = default!;
        public List<MovieViewRecord> ViewRecords { get; set; }
        public double AverageRating { get; set; }
        public bool UserHasRated { get; set; } = false;

        [BindProperty]
        public int UserRating { get; set; } // Rating from the form input


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var movie = await _context.Movie
                .Include(m => m.MovieRatings) // Include Ratings
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            DateTime today = DateTime.UtcNow.Date; //Get Today’s Date

            // Update View Count
            var viewRecord = await _context.MovieViewRecords
                .FirstOrDefaultAsync(v => v.MovieId == movie.Id && v.ViewDate == today); //Check if a view record exists for today

            if (viewRecord == null)
            {
                viewRecord = new MovieViewRecord { MovieId = movie.Id, ViewDate = today, ViewCount = 1 };
                _context.MovieViewRecords.Add(viewRecord);
            }
            else
            {
                viewRecord.ViewCount++;
            }

            movie.Views++;
            await _context.SaveChangesAsync();

            // chart data
            ViewRecords = await _context.MovieViewRecords
                        .Where(v => v.MovieId == id)
                        .OrderBy(v => v.ViewDate)
                        .ToListAsync(); // Fetch View Records for the Chart


            // Calculate Average Rating
            if (movie.MovieRatings.Any())
            {
                AverageRating = movie.MovieRatings.Average(r => r.Rating);
            }

            MovieObj = movie;
            return Page();
        }
        public async Task<IActionResult> OnPostRateAsync(int movieId)
        {
            var movie = await _context.Movie
                .Include(m => m.MovieRatings)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var userId = User.Identity.Name; // Get authenticated user's name
            var existingRating = await _context.MovieRatings
                .FirstOrDefaultAsync(r => r.MovieId == movieId && r.UserId == userId); //Check if the User Has Already Rated the Movie

            if (existingRating != null)
            {
                existingRating.Rating = UserRating; // Update existing rating
            }
            else
            {
                var newRating = new MovieRating
                {
                    MovieId = movieId,
                    UserId = userId,
                    Rating = UserRating
                };
                _context.MovieRatings.Add(newRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id = movieId }); // Refresh the page
        }

    }
}
