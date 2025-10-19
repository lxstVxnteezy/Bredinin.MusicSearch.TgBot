using Bredinin.MusicSearch.TgBot.Models.Entities;

namespace Bredinin.MusicSearch.TgBot.Core.Strategy;

public interface IAudioDownloadStrategy
{
    Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default);
    bool CanHandle(string url);
}