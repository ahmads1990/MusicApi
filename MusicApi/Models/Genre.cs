using MusicApi.Models.Join;
using System.Text.Json.Serialization;

namespace MusicApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        // Nav prop
        [JsonIgnore]
        public ICollection<Track> Tracks { get; set; }
    }
}
