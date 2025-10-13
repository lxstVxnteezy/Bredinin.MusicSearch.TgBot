using Bredinin.MusicSearch.TgBot.Models.Entities;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Strategy;

public interface IAudioDownloadStrategy
{
    Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default);
    bool CanHandle(string url);
}