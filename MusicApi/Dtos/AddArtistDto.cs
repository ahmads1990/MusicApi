namespace MusicApi.Dtos
{
    public class AddArtistDto
    {
        public string Name { get; set; }
        public string Bio { get; set; }
        public IFormFile ArtistCoverImage { get; set; }
    }
}
