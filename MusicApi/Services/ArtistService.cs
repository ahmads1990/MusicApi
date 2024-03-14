
namespace MusicApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly AppDbContext _dbContext;
        public ArtistService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<IEnumerable<Artist>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Artist GetById(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Artist> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Artist> SearchByNameAsync(string Name)
        {
            throw new NotImplementedException();
        }
        public Task<bool> CheckArtistExistAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Artist> CreateNewArtist(Artist newArtist)
        {
            throw new NotImplementedException();
        }








        public Artist UpdateArtist(Artist artist)
        {
            throw new NotImplementedException();
        }
        public Artist DeleteArtistk(Artist artist)
        {
            throw new NotImplementedException();
        }


    }
}
