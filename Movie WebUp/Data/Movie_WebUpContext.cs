using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Movie_WebUp.Model;

namespace Movie_WebUp.Data
{
    public class Movie_WebUpContext : IdentityDbContext<IdentityUser>

    {
        public Movie_WebUpContext (DbContextOptions<Movie_WebUpContext> options)
            : base(options)
        {
        }

        public DbSet<Movie_WebUp.Model.Movie> Movie { get; set; } = default!;
        public DbSet<MovieViewRecord> MovieViewRecords { get; set; } = default!;
        public DbSet<MovieRating> MovieRatings { get; set; }


    }
}
