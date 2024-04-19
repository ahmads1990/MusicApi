namespace MusicApi.Helpers.Config.FilesConfig
{
    public class FileServiceConfig
    {
        public List<string> AllowedFileExtensions { get; set; } = default!;
        public int FileMaxSizeMB { get; set; }
        public string SaveDirectory { get; set; } = string.Empty;
    }
}
