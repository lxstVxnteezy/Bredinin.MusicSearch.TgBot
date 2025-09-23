using Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Polling;
using YoutubeDLSharp;

namespace Bredinin.MusicSearchEngine.TgBot.Services
{
    public static class DependenciesExtensions
    {
        public static IServiceCollection AddMainServices(this IServiceCollection services)
        {
            services.AddScoped<IUpdateHandler, UpdateHandler>();
            
            services.AddSingleton<ITelegramBotService, TelegramBotService>();
            
            services.AddScoped(_ =>
            {
                var ytdl = new YoutubeDL();
                ytdl.YoutubeDLPath = "yt-dlp.exe";

                return ytdl;
            });
            
            services.AddScoped<IAudioDownloadService, AudioDownloadService>();

            return services;
        }
    }
}
