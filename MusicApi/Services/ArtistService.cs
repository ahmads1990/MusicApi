using Microsoft.EntityFrameworkCore;

namespace MusicApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly AppDbContext _dbContext;

        public ArtistService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Artist>> GetAllAsync()
        {
            return await _dbContext.Artists
                .ToListAsync();
        }

        public Artist? GetById(int id)
        {
            return _dbContext.Artists
                .FirstOrDefault(a => a.Id == id);
        }

        public async Task<Artist?> GetByIdAsync(int id)
        {
            return await _dbContext.Artists
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Artist?> SearchByNameAsync(string name)
        {
            return await _dbContext.Artists
                .FirstOrDefaultAsync(a => a.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> CheckArtistExistAsync(int id)
        {
            return await _dbContext.Artists
                .AnyAsync(a => a.Id == id);
        }

        public async Task<Artist> CreateNewArtist(Artist newArtist)
        {
            if (newArtist == null || string.IsNullOrEmpty(newArtist.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (newArtist.Id != 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            // Cover image
            //TODO
            var createdArtist = await _dbContext.Artists
                .AddAsync(newArtist);
            await _dbContext.SaveChangesAsync();

            return createdArtist.Entity;
        }

        public async Task<Artist?> UpdateArtistAsync(Artist artist)
        {
            if (artist == null || string.IsNullOrEmpty(artist.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (artist.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            bool doesArtistExist = await CheckArtistExistAsync(artist.Id);
            if (!doesArtistExist) return null;

            var updatedArtist = _dbContext.Artists
                .Update(artist);
            await _dbContext.SaveChangesAsync();

            return updatedArtist.Entity;
        }

        public async Task<Artist?> DeleteArtistAsync(Artist artist)
        {
            if (artist == null)
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (artist.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            bool doesArtistExist = await CheckArtistExistAsync(artist.Id);
            if (!doesArtistExist) return null;

            var deletedArtist = _dbContext.Artists
                .Remove(artist);
            await _dbContext.SaveChangesAsync();

            return deletedArtist.Entity;
        }
    }
}
