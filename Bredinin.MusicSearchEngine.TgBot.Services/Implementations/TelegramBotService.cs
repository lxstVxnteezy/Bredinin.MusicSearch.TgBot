using Bredinin.MusicSearch.TgBot.Models.Entities;
using Bredinin.MusicSearchEngine.TgBot.Services.Interfaces;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace Bredinin.MusicSearchEngine.TgBot.Services.Implementations
{
    public class TelegramBotService(
        IOptions<TelegramBotSettings> botOptions,
        IUpdateHandler updateHandler)
        : ITelegramBotService
    {
        private readonly TelegramBotClient _botClient = new(botOptions.Value.Token
                                                            ?? throw new InvalidOperationException());

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = []
            };

            _botClient.StartReceiving(
                updateHandler.HandleUpdateAsync,
                updateHandler.HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );

            var me = await _botClient.GetMe(cancellationToken);

            Console.WriteLine($"Бот @{me.Username} запущен!");
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
