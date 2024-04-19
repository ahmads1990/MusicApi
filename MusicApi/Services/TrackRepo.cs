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
            return await _dbContext.Tracks
                                .Include(t=>t.Genres)
                                .AsNoTracking()
                                .ToListAsync();
        }
        public Track? GetById(int id)
        {
            return _dbContext.Tracks
                .Include(t=>t.Genres)
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);
        }
        public async Task<Track?> GetByIdAsync(int id)
        {
            return await _dbContext.Tracks
                .Include(t => t.Genres)
                .FirstOrDefaultAsync(t => t.Id == id);       
        }
        public bool CheckTrackExist(int id)
        {
            return _dbContext.Tracks.Any(t => t.Id == id);
        }
        public async Task<Track> CreateNewTrack(Track newTrack)
        {
            if (newTrack == null || string.IsNullOrEmpty(newTrack.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (newTrack.Id != 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);            

            var createdTrack = await _dbContext.Tracks.AddAsync(newTrack);
            await _dbContext.SaveChangesAsync();

            return createdTrack.Entity;
        }
        public Track? UpdateTrack(Track track)
        {
            if (track == null || string.IsNullOrEmpty(track.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (track.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            if (!CheckTrackExist(track.Id)) return null;

            var updatedTrack = _dbContext.Tracks.Update(track);
            _dbContext.SaveChanges();

            return updatedTrack.Entity;
        }
        public Track? DeleteTrack(Track track)
        {
            if (track == null || string.IsNullOrEmpty(track.Name))
                throw new ArgumentException(ExceptionMessages.InvalidEntityData);

            if (track.Id <= 0)
                throw new ArgumentException(ExceptionMessages.InvalidEntityId);

            if (!CheckTrackExist(track.Id)) return null;

            var deletedTrack = _dbContext.Tracks.Remove(track);
            _dbContext.SaveChanges();

            return deletedTrack.Entity;
        }
    }
}
