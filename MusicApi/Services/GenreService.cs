using MusicApi.Repositories;
using MusicApi.Repositories.Interfaces;

namespace MusicApi.Services;

public class GenreService : IGenreService
{
    private readonly IGenreRepo _genreRepo;

    public GenreService(IGenreRepo genreRepo)
    {
        _genreRepo = genreRepo;
    }

    public async Task<IEnumerable<Genre>> GetAllAsync()
    {
        return await _genreRepo.GetAllAsync();
    }
    public async Task<IEnumerable<Genre>> GetAllWithIdAsync(IEnumerable<int> genreIds)
    {
        return await _genreRepo.GetAllWithIdAsync(genreIds);
    }
    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _genreRepo.GetByIdAsync(id);
    }
    public bool CheckGenreExist(int id)
    {
        return _genreRepo.CheckGenreExist(id);
    }
    public Task<Genre> CreateNewGenre(Genre newGenre)
    {
        return _genreRepo.CreateNewGenre(newGenre);
    }
    public Genre? UpdateGenre(Genre genre)
    {
        return _genreRepo.UpdateGenre(genre);
    }
    public Genre? DeleteGenre(Genre genre)
    {
        return _genreRepo.DeleteGenre(genre);
    }
}
