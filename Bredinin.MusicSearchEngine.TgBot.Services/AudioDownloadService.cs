using Bredinin.MusicSearch.TgBot.Models;
using Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;
using YoutubeDLSharp.Options;
using YoutubeDLSharp;
using Microsoft.Extensions.Options;

namespace Bredinin.MusicSearchEngine.TgBot.Services
{
    public class AudioDownloadService : IAudioDownloadService
    {
        private readonly YoutubeDL _ytdl;
        private readonly string _downloadFolder;

        public AudioDownloadService(IOptions<DownloadSettings> downloadOptions, YoutubeDL ytdl)
        {
            _ytdl = ytdl;

            _downloadFolder = Path.Combine(AppContext.BaseDirectory, downloadOptions.Value.DownloadPath);

            Directory.CreateDirectory(_downloadFolder);
        }

        public async Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var metadataResult = await _ytdl.RunVideoDataFetch(url, overrideOptions: new OptionSet
                {
                    SkipDownload = true
                }, ct: cancellationToken);

                string? title = null;
                string? thumbnailUrl = null;

                if (metadataResult.Success && metadataResult.Data is { } data)
                {
                    title = data.Title;
                    thumbnailUrl = data.Thumbnail;
                }

                var safeFileName = $"{Guid.NewGuid()}.m4a";
                var outputPath = Path.Combine(_downloadFolder, safeFileName);

                var result = await _ytdl.RunAudioDownload(
                    url,
                    AudioConversionFormat.M4a,
                    cancellationToken,
                    overrideOptions: new OptionSet
                    {
                        Output = outputPath,
                        ExtractAudio = true,
                    });


                if (!result.Success || string.IsNullOrWhiteSpace(result.Data))
                {
                    return new DownloadResult
                    {
                        Success = false,
                        ErrorMessage = result.ErrorOutput?.FirstOrDefault()
                    };
                }

                return new DownloadResult
                {
                    Success = true,
                    FilePath = outputPath,
                    Title = title,
                    ThumbnailUrl = thumbnailUrl
                };
            }
            catch (Exception ex)
            {
                return new DownloadResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
