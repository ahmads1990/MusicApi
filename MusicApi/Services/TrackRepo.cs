using MusicApi.Services.Interfaces;

namespace MusicApi.Services
{
    public class TrackRepo : ITrackRepo
    {
        private readonly AppDbContext _dbContext;
        public TrackRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Task<IEnumerable<Track>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
        public Task<Track> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<Track> CreateNewTrack(Track newTrack)
        {
            throw new NotImplementedException();
        }
        public Task<Track> UpdateTrack(Track updatedTrack)
        {
            throw new NotImplementedException();
        }
        public Task<Track> DeleteTrack(Track toDeleteTrack)
        {
            throw new NotImplementedException();
        }            
    }
}
