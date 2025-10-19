using Bredinin.MusicSearch.TgBot.Core.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Bredinin.MusicSearch.TgBot;

public class BotBackgroundService(ITelegramBotService botService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) => 
        await botService.StartAsync(stoppingToken);
}

