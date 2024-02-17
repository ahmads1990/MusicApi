namespace MusicApi.Services.FileServices
{
    public abstract class FileServiceBase
    {
        public abstract List<string> AllowedFileExtensions { get; set; }
        public abstract int FileMaxSizeMB { get; set; }
        public abstract string SaveDirectory { get; set; }
        // Check for file then save file in path using (entityType) art
        public abstract string SaveFileAndCheckFile(IFormFile file, string entityType);
        // Check for file size and Extensions
        public  bool CheckFileSpecs(IFormFile file)
        {
            // Check file size
            float fileSize = ByteToMegaByte(file.Length);
            if (fileSize == 0 || fileSize > FileMaxSizeMB)
                return false;

            // Check file extention
            string fileExtension = file.ContentType.Split("/").Last();
            if (!AllowedFileExtensions.Contains(fileExtension))
                return false;

            return true;
        }  
        protected float ByteToMegaByte(float bytes) => bytes / 1000000;
    }
}
