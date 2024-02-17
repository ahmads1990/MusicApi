
namespace MusicApi.Services.FileServices
{
    public class ImageService : FileServiceBase
    {
        public override List<string> AllowedFileExtensions { get; set; }
        public override int FileMaxSizeMB { get; set; }
        public override string SaveDirectory { get; set; }
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ImageService(IWebHostEnvironment webHostEnvironment)
        {
            AllowedFileExtensions = new List<string> { ".jpg", ".png", };
            FileMaxSizeMB = 2;
            SaveDirectory = "Images";
            _webHostEnvironment = webHostEnvironment;   
        }
        public override string SaveFileAndCheckFile(IFormFile file, string entityType)
        {
            // Check file specs if doesn't meet req return empty string 
            if (!CheckFileSpecs(file))
                return string.Empty;
            // Save path = "rootDir/SaveDirectory/entityType/fileName"
            string rootPath = _webHostEnvironment.WebRootPath;
            string saveDirectory = Path.Combine(rootPath,SaveDirectory, entityType);

            // Ensure the save directory exists; create it if not
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            string savePath = Path.Combine(saveDirectory, file.FileName);

            // Save the file
            using (var stream = new FileStream(savePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return savePath;
        }
    }
}
