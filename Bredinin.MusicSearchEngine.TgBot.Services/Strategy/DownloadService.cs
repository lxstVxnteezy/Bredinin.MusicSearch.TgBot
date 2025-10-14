using Bredinin.MusicSearch.TgBot.Models.Entities;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Strategy;

public class DownloadService(IEnumerable<IAudioDownloadStrategy> strategies)
{
    public async Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default)
    {
        var strategy = strategies.SingleOrDefault(d => d.CanHandle(url));

        if (strategy == null)
            throw new InvalidOperationException($"Not handle for URL: {url}");

        return await strategy.DownloadAudioAsync(url, cancellationToken);
    }
}