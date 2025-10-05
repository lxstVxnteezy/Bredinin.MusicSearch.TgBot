using Bredinin.MusicSearch.TgBot.Models.Entities;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Interfaces
{
    public interface IAudioDownloadService
    {
        Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default);
    }
}
