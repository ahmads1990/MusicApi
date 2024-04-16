using System.ComponentModel.DataAnnotations;

namespace MusicApi.Dtos
{
    public class AddTrackDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }
        //[Required]
        public IFormFile TrackFile { get; set; }
        [Required]
        // Genre refrence
        public IEnumerable<int> Genres { get; set; }
    }
}
