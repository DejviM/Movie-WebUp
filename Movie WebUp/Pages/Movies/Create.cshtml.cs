using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Movie_WebUp.Data;
using Movie_WebUp.Model;

namespace Movie_WebUp.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly Movie_WebUp.Data.Movie_WebUpContext _context;

        public CreateModel(Movie_WebUp.Data.Movie_WebUpContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty] //Allow data binding that is to bind the form data to the Movie object from UI
        public Movie MovieObj { get; set; } = default!;

        [BindProperty]
        public IFormFile PhotoUpload { get; set; } // For file upload

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

            if (PhotoUpload != null && PhotoUpload.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await PhotoUpload.CopyToAsync(memoryStream);
                    MovieObj.Photo = memoryStream.ToArray(); // Save the photo as a byte array
                    MovieObj.PhotoContentType = PhotoUpload.ContentType; // Save the MIME type defines what format is the file in our case is a photo
                }
            }

            // Set the creation timestamp
            MovieObj.CreatedAt = DateTime.UtcNow;

            // Add the film to the database
            _context.Movie.Add(MovieObj);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Movie_List");
        }
    }
}
