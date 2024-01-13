namespace MusicApi.Dtos
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Nav prop
        public IEnumerable<GenreDto> Genres { get; set; }
    }
}
