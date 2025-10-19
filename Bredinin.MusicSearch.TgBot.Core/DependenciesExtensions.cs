using Bredinin.MusicSearch.TgBot.Core.Interfaces;
using Bredinin.MusicSearch.TgBot.Core.Strategy;
using Bredinin.MusicSearch.TgBot.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot.Polling;
using YoutubeDLSharp;

namespace Bredinin.MusicSearch.TgBot.Core;

public static class DependenciesExtensions
{
    public static IServiceCollection AddMainServices(this IServiceCollection services)
    {
        services.AddScoped<IUpdateHandler, UpdateHandler>();

        services.AddSingleton<ITelegramBotService, TelegramBotService>();

        services.AddScoped(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<DownloadSettings>>().Value;

            var ytdl = new YoutubeDL();

            ytdl.YoutubeDLPath = settings.YoutubeDlPath;

            return ytdl;
        });

        services.AddScoped<IAudioDownloadStrategy, YoutubeDownloadStrategy>();

        services.AddTransient<DownloadService>();

      
        return services;
    }
}