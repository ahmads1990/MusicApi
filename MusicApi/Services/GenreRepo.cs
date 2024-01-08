
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
            if (newGenre == null || newGenre.Id != 0 || newGenre.Name == null)
                throw new ArgumentException("Invalid Genre data");

            var createdGenre = await _dbContext.Genres.AddAsync(newGenre);
            await _dbContext.SaveChangesAsync();

            return createdGenre.Entity;
        }
        public Genre UpdateGenre(Genre genre)
        {
            if (genre == null || genre.Id == 0 || genre.Name == null)
                throw new ArgumentException("Invalid Genre data");


            if (!CheckGenreExist(genre.Id))
                throw new ArgumentException("Genre does not exist");

            var updatedGenre = _dbContext.Genres.Update(genre);
            _dbContext.SaveChanges();

            return updatedGenre.Entity;
        }
        public Genre DeleteGenre(Genre genre)
        {
            if (genre == null || genre.Id == 0 || genre.Name == null)
                throw new ArgumentException("Invalid Genre data");


            if (!CheckGenreExist(genre.Id))
                throw new ArgumentException("Genre does not exist");

            var updatedGenre = _dbContext.Genres.Remove(genre);
            _dbContext.SaveChanges();

            return updatedGenre.Entity;
        }
    }
}
