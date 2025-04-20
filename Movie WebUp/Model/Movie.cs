using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Movie_WebUp.Model
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Release Date"), DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public string Description { get; set; }


        [Range(1, 100), DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [RegularExpression(@"^[A-Z]+[a-zA-Z\s]*$"), Required, ]
        public string Genre { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public byte[]? Photo { get; set; }
        public string? PhotoContentType { get; set; }

        public int Views { get; set; } = 0; // Track movie views

        public double AverageRating { get; set; }

        public ICollection<MovieRating> MovieRatings { get; set; } = new List<MovieRating>(); //one to many 
    }
}
