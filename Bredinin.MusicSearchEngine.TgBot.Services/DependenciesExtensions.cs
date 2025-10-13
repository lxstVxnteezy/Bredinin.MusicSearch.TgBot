using Bredinin.MusicSearch.TgBot.Models.Entities;
using Bredinin.MusicSearchEngine.TgBot.Services.Implementations;
using Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;
using Bredinin.MusicSearchEngine.TgBot.Services.Strategy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot.Polling;
using YoutubeDLSharp;

namespace Bredinin.MusicSearchEngine.TgBot.Services;

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