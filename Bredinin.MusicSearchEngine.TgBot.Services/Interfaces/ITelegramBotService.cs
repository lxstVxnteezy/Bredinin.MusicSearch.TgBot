namespace Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;

public interface ITelegramBotService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);
}

