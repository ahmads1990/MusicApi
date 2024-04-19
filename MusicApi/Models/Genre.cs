using System.Text.Json.Serialization;

namespace MusicApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        // Nav prop
        [JsonIgnore]
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
