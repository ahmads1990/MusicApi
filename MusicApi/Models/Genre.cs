using MusicApi.Models.Join;
using System.Text.Json.Serialization;

namespace MusicApi.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Nav prop
        [JsonIgnore]
        public IEnumerable<Track> Tracks { get; set; }
    }
}
