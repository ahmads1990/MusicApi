
using MusicApi.Repositories.Interfaces;

namespace MusicApi.Services;

public class TrackService : ITrackService
{
    private readonly ITrackRepo _trackRepo;

    public TrackService(ITrackRepo trackRepo)
    {
        _trackRepo = trackRepo;
    }

    public async Task<IEnumerable<Track>> GetAllAsync()
    {
        return await _trackRepo.GetAllAsync();
    }
    public Track? GetById(int id)
    {
        return _trackRepo.GetById(id);
    }
    public async Task<Track?> GetByIdAsync(int id)
    {
        return await _trackRepo.GetByIdAsync(id);
    }
    public bool CheckTrackExist(int id)
    {
        return _trackRepo.CheckTrackExist(id);
    }

    public async Task<Track> CreateNewTrack(Track newTrack)
    {
        return await _trackRepo.CreateNewTrack(newTrack);
    }
    public Track? UpdateTrack(Track track)
    {
        return _trackRepo.UpdateTrack(track);
    }
    public Track? DeleteTrack(Track track)
    {
        return _trackRepo.DeleteTrack(track);
    }
}
