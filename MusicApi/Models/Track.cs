using MusicApi.Models.Join;

namespace MusicApi.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Nav prop
        public ICollection<Genre> Genres { get; set; }
    }
}
