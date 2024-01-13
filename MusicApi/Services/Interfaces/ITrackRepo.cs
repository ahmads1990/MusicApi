namespace MusicApi.Services.Interfaces
{
    public interface ITrackRepo
    {
        public Task<IEnumerable<Track>> GetAllAsync();
        public Track GetById(int id);
        public Task<Track> GetByIdAsync(int id);
        public bool CheckTrackExist(int id);
        public Task<Track> CreateNewTrack(Track newTrack);
        public Track UpdateTrack(Track track);
        public Track DeleteTrack(Track track);
    }
}
