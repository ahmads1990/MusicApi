using MusicApi.Helpers;
using MusicApi.Helpers.Config.FilesConfig;

namespace MusicApi.Services.FileServices
{
    public interface IFileService
    {
        string SaveFileAndCheckFile(IFormFile file, FileTypes fileType);
        bool CheckFileSpecs(IFormFile file, FileServiceConfig config);
    }
}
