using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using MimeKit;
using MusicApi.Helpers;
using MusicApi.Helpers.Config.FilesConfig;

namespace MusicApi.Services.FileServices
{
    public class FileService : IFileService
    {
        // To do reserach more about options
        private readonly AlbumImageFileConfig _albumImageFileConfig;
        private readonly UserImageFileConfig _userImageFileConfig;
        private readonly TrackFileConfig _trackFileConfig;

        private readonly IWebHostEnvironment _webHostEnvironment;
        public FileService(IOptions<AlbumImageFileConfig> albumImageFileConfig, IOptions<UserImageFileConfig> userImageFileConfig, IOptions<TrackFileConfig> trackFileConfig, IWebHostEnvironment webHostEnvironment)
        {
            _albumImageFileConfig = albumImageFileConfig.Value;
            _userImageFileConfig = userImageFileConfig.Value;
            _trackFileConfig = trackFileConfig.Value;
            _webHostEnvironment = webHostEnvironment;
        }
        public string SaveFileAndCheckFile(IFormFile file, FileTypes fileType)
        {
            FileServiceConfig config = GetConfigForFileType(fileType);

            // Check file specs if doesn't meet req return empty string 
            if (!CheckFileSpecs(file, config))
                return string.Empty;
            // Save path = "rootDir/SaveDirectory/entityType/fileName"
            string rootPath = _webHostEnvironment.WebRootPath;
            string saveDir = Path.Combine(rootPath, config.SaveDirectory);

            // Ensure the save directory exists; create it if not
            if (!Directory.Exists(saveDir))
            {
                Directory.CreateDirectory(saveDir);
            }

            string savePath = Path.Combine(saveDir, file.FileName);

            // Save the file
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return savePath;
        }
        public bool CheckFileSpecs(IFormFile file, FileServiceConfig config)
        {
            // Check file size
            float fileSize = ByteToMegaByte(file.Length);
            if (fileSize == 0 || fileSize > config.FileMaxSizeMB)
                return false;


            // Check file extention
            if (MimeTypes.TryGetExtension(file.ContentType, out var fileExtension))
            {
                if (!config.AllowedFileExtensions.Contains(fileExtension))
                    return false;
            }
            else 
                return false;
            

            // Extra for each type

            return true;
        }
        
        protected float ByteToMegaByte(float bytes) => bytes / 1000000;
        private FileServiceConfig GetConfigForFileType(FileTypes fileType)
        {
            switch (fileType)
            {
                case FileTypes.UserImage:
                    return _userImageFileConfig;
                case FileTypes.TrackFile:
                    return _trackFileConfig;
                case FileTypes.AlbumImage:
                    return _albumImageFileConfig;
                default:
                    throw new ArgumentException("Invalid file type.");
            }
        }
    }
}
