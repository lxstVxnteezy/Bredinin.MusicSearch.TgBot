using Bredinin.MusicSearch.TgBot.Models.Entities;
using Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;
using Microsoft.Extensions.Options;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Implementations
{
    public class AudioDownloadService : IAudioDownloadService
    {
        private readonly YoutubeDL _ytdl;
        private readonly string _downloadFolder;

        public AudioDownloadService(
            IOptions<DownloadSettings> downloadOptions,
            YoutubeDL ytdl)
        {
            _ytdl = ytdl;

            _downloadFolder = Path.Combine
                (AppContext.BaseDirectory, downloadOptions.Value.DownloadPath);

            Directory.CreateDirectory(_downloadFolder);
        }

        public async Task<DownloadResult> DownloadAudioAsync(string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var safeFileName = $"{Guid.NewGuid()}.m4a";

                var outputPath = Path.Combine(_downloadFolder, safeFileName);

                var metadataTask = _ytdl.RunVideoDataFetch(
                    url,
                    overrideOptions: new OptionSet
                    {
                        SkipDownload = true
                    },
                    ct: cancellationToken
                );

                var audioTask = _ytdl.RunAudioDownload(
                    url,
                    AudioConversionFormat.M4a,
                    cancellationToken,
                    overrideOptions: new OptionSet
                    {
                        Output = outputPath,
                        ExtractAudio = true,
                    }
                );

                await Task.WhenAll(metadataTask, audioTask);

                var metadataResult = metadataTask.Result;
                var audioResult = audioTask.Result;

                if (!metadataResult.Success)
                {
                    return new DownloadResult
                    {
                        Success = metadataResult.Success,
                        ErrorMessage = metadataResult.ErrorOutput?.FirstOrDefault()
                    };
                }

                if (!audioResult.Success || string.IsNullOrWhiteSpace(audioResult.Data))
                    return new DownloadResult
                    {
                        Success = false,
                        ErrorMessage = audioResult.ErrorOutput?.FirstOrDefault()
                    };

                return new DownloadResult
                {
                    Success = true,
                    FilePath = outputPath,
                    Title = metadataResult.Data.Title,
                    ThumbnailUrl = metadataResult.Data.Thumbnail
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
