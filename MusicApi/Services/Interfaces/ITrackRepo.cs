namespace MusicApi.Services.Interfaces
{
    public interface ITrackRepo
    {
        public Task<IEnumerable<Track>> GetAllAsync();
        public Task<Track> GetByIdAsync(int id);
        public Task<Track> CreateNewTrack(Track newTrack);
        public Task<Track> UpdateTrack(Track updatedTrack);
        public Task<Track> DeleteTrack(Track toDeleteTrack);
    }
}
