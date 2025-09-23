namespace Bredinin.MusicSearch.TgBot.Models;
public class DownloadResult
{
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? Title { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

