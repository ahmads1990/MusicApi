namespace MusicApi.Dtos
{
    public class ArtistDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Biography { get; set; } = string.Empty;
        public int Followers { get; set; }
        public string CoverPath { get; set; } = string.Empty;
    }
}
