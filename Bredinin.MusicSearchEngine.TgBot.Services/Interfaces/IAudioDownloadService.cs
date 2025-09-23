using Bredinin.MusicSearch.TgBot.Models;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Interfaces
{
    public interface IAudioDownloadService
    {
        Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default);
    }
}
