using System.ComponentModel.DataAnnotations;

namespace Movie_WebUp.Model
{
    public class MovieRating
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty; // To store the authenticated user

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Movie Movie { get; set; } //Many rating to one movie 
    }
}
