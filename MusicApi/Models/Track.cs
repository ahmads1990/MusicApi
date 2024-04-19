using MusicApi.Models.Join;

namespace MusicApi.Models
{
    public class Track
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TrackPath { get; set; } = string.Empty;
        public int LengthInSeconds { get; set; }
        public DateTime ReleaseDate { get; set; }
        // Genre refrencep
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    }
}
