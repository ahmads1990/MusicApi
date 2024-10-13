
using MusicApi.Repositories.Interfaces;

namespace MusicApi.Services;

public class ArtistService : IArtistService
{
    private readonly IArtistRepo _artistRepo;

    public ArtistService(IArtistRepo artistRepo)
    {
        _artistRepo = artistRepo;
    }

    public async Task<IEnumerable<Artist>> GetAllAsync()
    {
        return await _artistRepo.GetAllAsync();
    }
    public Artist? GetById(int id)
    {
        return _artistRepo.GetById(id);
    }
    public Task<Artist?> GetByIdAsync(int id)
    {
        return _artistRepo.GetByIdAsync(id);
    }
    public async Task<Artist?> SearchByNameAsync(string Name)
    {
        return await _artistRepo.SearchByNameAsync(Name);
    }
    public async Task<bool> CheckArtistExistAsync(int id)
    {
        return await _artistRepo.CheckArtistExistAsync(id);
    }
    public async Task<Artist> CreateNewArtist(Artist newArtist)
    {
        return await _artistRepo.CreateNewArtist(newArtist);
    }
    public async Task<Artist?> UpdateArtistAsync(Artist artist)
    {
        return await _artistRepo.UpdateArtistAsync(artist);
    }
    public async Task<Artist?> DeleteArtistAsync(Artist artist)
    {
        return await _artistRepo.DeleteArtistAsync(artist);
    }
}
