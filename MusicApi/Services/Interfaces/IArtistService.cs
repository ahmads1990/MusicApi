namespace MusicApi.Services.Interfaces;

public interface IArtistService
{
    public Task<IEnumerable<Artist>> GetAllAsync();
    public Artist? GetById(int id);
    public Task<Artist?> GetByIdAsync(int id);
    public Task<Artist?> SearchByNameAsync(string Name);
    public Task<bool> CheckArtistExistAsync(int id);
    public Task<Artist> CreateNewArtist(Artist newArtist);
    public Task<Artist?> UpdateArtistAsync(Artist artist);
    public Task<Artist?> DeleteArtistAsync(Artist artist);
}
