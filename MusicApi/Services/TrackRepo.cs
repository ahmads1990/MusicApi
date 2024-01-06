using Microsoft.EntityFrameworkCore;

namespace MusicApi.Services
{
    public class TrackRepo : ITrackRepo
    {
        private readonly AppDbContext _dbContext;
        public TrackRepo(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Track>> GetAllAsync()
        {
            return await _dbContext.Tracks.ToListAsync();
        }
        public async Task<Track> GetByIdAsync(int id)
        {
            return await _dbContext.Tracks.FirstOrDefaultAsync(t => t.Id == id);
        }
        public bool CheckTrackExist(int id)
        {
            return _dbContext.Tracks.Any(t => t.Id == id);
        }
        public async Task<Track> CreateNewTrack(Track newTrack)
        {
            if (newTrack == null || newTrack.Id != 0 || newTrack.Name == null)
                throw new ArgumentException("Invalid track data");

            var createdTrack = await _dbContext.Tracks.AddAsync(newTrack);
            await _dbContext.SaveChangesAsync();

            return createdTrack.Entity;
        }
        public Track UpdateTrack(Track track)
        {
            if (track == null || track.Id == 0 || track.Name == null)
                throw new ArgumentException("Invalid track data");

            if (!CheckTrackExist(track.Id))
                throw new ArgumentException("Track does not exist");

            var updatedTrack = _dbContext.Tracks.Update(track);
            _dbContext.SaveChanges();

            return updatedTrack.Entity;
        }
        public Track DeleteTrack(Track track)
        {
            if (track == null || track.Id == 0 || track.Name == null)
                throw new ArgumentException("Invalid track data");

            if (!CheckTrackExist(track.Id))
                throw new ArgumentException("Track does not exist");

            var deletedTrack = _dbContext.Tracks.Remove(track);
            _dbContext.SaveChanges();

            return deletedTrack.Entity;
        }
    }
}
