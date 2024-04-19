using System.ComponentModel.DataAnnotations;

namespace MusicApi.Dtos
{
    public class AddTrackDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime ReleaseDate { get; set; }
        //[Required]
        public IFormFile TrackFile { get; set; } = default!;
        [Required]
        // Genre refrence
        public IEnumerable<int> Genres { get; set; } = new List<int>();
    }
}
