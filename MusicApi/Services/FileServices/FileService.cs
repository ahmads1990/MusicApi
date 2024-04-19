using Microsoft.Extensions.Options;
using MimeKit;
using MusicApi.Helpers;
using MusicApi.Helpers.Config.FilesConfig;
using System.Diagnostics;

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
        public async Task<string> SaveImageFile(IFormFile file, FileTypes fileType)
        {
            FileServiceConfig config = GetConfigForFileType(fileType);

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
                await file.CopyToAsync(stream);
            }
            return savePath;
        }
        public async Task<TrackFileSaveDto> SaveTrackFileHLS(IFormFile file, string trackNameWithoutExtension)
        {
            TrackFileConfig config = _trackFileConfig;
            TrackFileSaveDto fileSaveDto = new TrackFileSaveDto();

            // Save path for original file = "rootDir/Uploads/entity/Tracks/trackname.mp3"
            // Save path for hls file = "rootDir/Uploads/entity/Tracks/hls/trackname/trackname.m3u8"
            string rootPath = _webHostEnvironment.WebRootPath;
            string saveDirOriginal = Path.Combine(rootPath, config.SaveDirectory);
            string saveDirHls = Path.Combine(rootPath, config.SaveDirectoryHLS, trackNameWithoutExtension);

            // Ensure the save directory exists; create it if not
            if (!Directory.Exists(saveDirOriginal)) Directory.CreateDirectory(saveDirOriginal);
            if (!Directory.Exists(saveDirHls)) Directory.CreateDirectory(saveDirHls);

            // Modify the file name (file) or leave it as is
            var fileName = trackNameWithoutExtension + Path.GetExtension(file.FileName);
            //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            // save path = dir + file name + Extension
            string savePathOriginal = Path.Combine(saveDirOriginal, fileName);

            try
            {
                // Save the original file first
                using (var stream = new FileStream(savePathOriginal, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                var trackDuration = await GetAudioFileDurationInSeconds(savePathOriginal);
                // Check on the duration
                if (trackDuration <= 0)
                    throw new Exception("Short track file");

                // Save the file in hls format
                var playlistTitle = await ConvertToHls(savePathOriginal, trackNameWithoutExtension, saveDirHls);

                // prepare the return dto
                fileSaveDto.isSaved = true;
                fileSaveDto.FileSavePathOriginal = savePathOriginal;
                fileSaveDto.FileDurationInSeconds = trackDuration;
                // The store the path in easier format to request
                fileSaveDto.FileSavePathHLS = $"{config.SaveDirectoryHLS}/{trackNameWithoutExtension}/{playlistTitle}";
            }
            catch (Exception)
            {
                throw new Exception("Error saving track file");
            }
            return fileSaveDto;
        }
        public bool CheckFileSpecs(IFormFile file, FileTypes fileType)
        {
            FileServiceConfig config = GetConfigForFileType(fileType);

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
        private async Task<string> ConvertToHls(string inputPath, string playlistFileName, string outputDir)
        {
            string playlistTitle = playlistFileName + ".m3u8";
            var playlistOutputPath = Path.Combine(outputDir, playlistTitle);
            var segmentName = Path.Combine(outputDir, "segment%05d.ts");

            var arguments = $"-i \"{inputPath}\" " + // input file
                          $"-vn -ac 2 -acodec aac " + // audio options
                          $"-f segment -segment_format mpegts -segment_time 10 " + // segment options
                          $"-segment_list \"{playlistOutputPath}\" \"{segmentName}\""; // segment list file and naming pattern

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            try
            {

                process.Start();
                process.BeginOutputReadLine();
                var errorTask = await Task.Run(() => process.StandardError.ReadToEndAsync());

                await process.WaitForExitAsync();
                return playlistTitle;

            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine($"Error: {ex.Message}");
                return string.Empty;
            }
        }
        private async Task<int> GetAudioFileDurationInSeconds(string filePath)
        {
            var audioFileDuration = -1;
            var arguments = $"-i \"{filePath}\" -show_entries format=duration -v quiet -of csv=\"p=0\"";

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffprobe",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            // Run the proccess
            try
            {
                process.Start();
                string outputDuration = await process.StandardOutput.ReadToEndAsync();

                await process.WaitForExitAsync();

                // parse the output from ffmpeg 
                audioFileDuration = Convert.ToInt32(Math.Floor(Convert.ToDecimal(outputDuration)));
                return audioFileDuration;
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine($"Error: {ex.Message}");
                return audioFileDuration;
            }
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
