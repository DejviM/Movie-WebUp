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
    public class EditModel : PageModel
    {
        private readonly Movie_WebUp.Data.Movie_WebUpContext _context;

        public EditModel(Movie_WebUp.Data.Movie_WebUpContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Movie MovieObj { get; set; } = default!;
        [BindProperty]
        public IFormFile? NewPhoto { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            MovieObj = await _context.Movie.FindAsync(id);
            if (MovieObj == null)
            {
                return NotFound();
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }
                return Page();
            }

            // Fetch the existing movie from the database
            var movieToUpdate = await _context.Movie.FindAsync(MovieObj.Id);
            if (movieToUpdate == null)
            {
                return NotFound();
            }

            // Update all properties of the movie except the photo (if no new photo is uploaded)
            movieToUpdate.Title = MovieObj.Title;
            movieToUpdate.ReleaseDate = MovieObj.ReleaseDate;
            movieToUpdate.Description = MovieObj.Description;
            movieToUpdate.Price = MovieObj.Price;
            movieToUpdate.Genre = MovieObj.Genre;

            // If a new photo is uploaded, save it
            if (NewPhoto != null)
            {

                using (var memoryStream = new MemoryStream()) // temp memory
                {
                    await NewPhoto.CopyToAsync(memoryStream);
                    movieToUpdate.Photo = memoryStream.ToArray();  // Store as byte array in DB
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();

            return RedirectToPage("./Movie_List");
        }
    }
}
