
using Microsoft.EntityFrameworkCore;
using MusicApi.Models;

namespace MusicApi.Services
{
    public class GenreRepo : IGenreRepo
    {
        private readonly AppDbContext _dbContext;
        public GenreRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _dbContext.Genres.ToListAsync();
        }
        public async Task<IEnumerable<Genre>> GetAllWithIdAsync(IEnumerable<int> genreIds)
        {
            return await _dbContext.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();
        }
        public async Task<Genre> GetByIdAsync(int id)
        {
            return await _dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }
        public bool CheckGenreExist(int id)
        {
            return _dbContext.Genres.Any(g => g.Id == id);
        }
        public async Task<Genre> CreateNewGenre(Genre newGenre)
        {
            if (newGenre == null || string.IsNullOrEmpty(newGenre.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (newGenre.Id != 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            var createdGenre = await _dbContext.Genres.AddAsync(newGenre);
            await _dbContext.SaveChangesAsync();

            return createdGenre.Entity;
        }
        public Genre UpdateGenre(Genre genre)
        {
            if (genre == null || string.IsNullOrEmpty(genre.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (genre.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            if (!CheckGenreExist(genre.Id)) return null;

            var updatedGenre = _dbContext.Genres.Update(genre);
            _dbContext.SaveChanges();

            return updatedGenre.Entity;
        }
        public Genre DeleteGenre(Genre genre)
        {
            if (genre == null || string.IsNullOrEmpty(genre.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (genre.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            if (!CheckGenreExist(genre.Id)) return null;

            var updatedGenre = _dbContext.Genres.Remove(genre);
            _dbContext.SaveChanges();

            return updatedGenre.Entity;
        }
    }
}
