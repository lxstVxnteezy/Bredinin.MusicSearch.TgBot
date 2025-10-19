using Bredinin.MusicSearch.TgBot;
using Bredinin.MusicSearch.TgBot.Core;
using Bredinin.MusicSearch.TgBot.Models.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<TelegramBotSettings>(
            context.Configuration.GetSection("TelegramBot"));

        services.Configure<DownloadSettings>(
            context.Configuration.GetSection("DownloadSettings"));

        services.AddMainServices();

        services.AddHostedService<BotBackgroundService>();
    })
    .Build();

await host.RunAsync();