using MusicApi.Helpers;

namespace MusicApi.Services.FileServices
{
    public interface IFileService
    {
        Task<string> SaveImageFile(IFormFile file, FileTypes fileType);
        Task<TrackFileSaveDto> SaveTrackFileHLS(IFormFile file, string trackNameWithoutExtension);
        bool CheckFileSpecs(IFormFile file, FileTypes fileType);
    }
}
