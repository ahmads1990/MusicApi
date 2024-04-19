namespace MusicApi.Dtos
{
    public class AddArtistDto
    {
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public IFormFile ArtistCoverImage { get; set; } = default!;
    }
}
