namespace MusicApi.Dtos
{
    public class TrackDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TrackPath { get; set; }
        public int Length { get; set; }
        public DateTime ReleaseDate { get; set; }
        // Genre refrencep
        public IEnumerable<GenreDto> Genres { get; set; }
    }
}
