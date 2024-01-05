using Microsoft.EntityFrameworkCore;
using MusicApi.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

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
        public async Task<Track> CreateNewTrack(Track newTrack)
        {
            if (newTrack == null || newTrack.Id != 0 || newTrack.Name == null)
                // throw new ArgumentException("Invalid track data");
                return null;

            var createdTrack = await _dbContext.Tracks.AddAsync(newTrack);
            await _dbContext.SaveChangesAsync();

            return createdTrack.Entity;
        }
        public Track UpdateTrack(Track track)
        {
            if (track == null || track.Id == 0 || track.Name == null)
                // throw new ArgumentException("Invalid track data");
                return null;

            var updatedTrack = _dbContext.Tracks.Update(track);
            _dbContext.SaveChanges();

            return updatedTrack.Entity;
        }
        public Track DeleteTrack(Track track)
        {
            if (track == null || track.Id == 0 || track.Name == null)
                // throw new ArgumentException("Invalid track data");
                return null;

            var deletedTrack = _dbContext.Tracks.Remove(track);
            _dbContext.SaveChanges();

            return deletedTrack.Entity;
        }
    }
}
