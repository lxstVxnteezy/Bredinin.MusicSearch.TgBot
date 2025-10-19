namespace Bredinin.MusicSearch.TgBot.Core.Interfaces;

public interface ITelegramBotService
{
    Task StartAsync(CancellationToken cancellationToken = default);
    
    Task StopAsync(CancellationToken cancellationToken = default);
}

