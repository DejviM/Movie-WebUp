using System.ComponentModel.DataAnnotations;

namespace Movie_WebUp.Model
{
    public class MovieViewRecord
    {
        [Key]
        public int Id { get; set; }
        public int MovieId { get; set; }
        public DateTime ViewDate { get; set; }
        public int ViewCount { get; set; }

        public Movie Movie { get; set; }
    }
}
