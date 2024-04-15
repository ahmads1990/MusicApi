namespace MusicApi.Dtos;

public class TrackFileSaveDto
{
    public bool isSaved { get; set; }
    public string FileSavePathOriginal { get; set; } = string.Empty;
    public string FileSavePathHLS { get; set; } = string.Empty;
    public int FileDurationInSeconds { get; set; }
}
