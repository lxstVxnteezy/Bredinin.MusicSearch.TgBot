using Bredinin.MusicSearchEngine.TgBot.Services.Strategy;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace Bredinin.MusicSearchEngine.TgBot.Services;

public class UpdateHandler(DownloadService downloadStrategy) : IUpdateHandler
{
    public async Task HandleUpdateAsync(
        ITelegramBotClient botClient,
        Update update,
        CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                if (update.Message is not { } message || message.Text is not { } messageText)
                    return;

                var chatId = message.Chat.Id;

                if (messageText.StartsWith("/"))
                    await HandleCommandAsync(botClient, message, cancellationToken);
                else if (IsValidUrl(messageText))
                    await HandleAudioDownloadAsync(botClient, message, messageText, cancellationToken);
                else
                    await botClient.SendMessage(
                        chatId,
                        "🎵 Привет! Отправь мне ссылку и я скачаю аудио.\n\nКоманды:\n/help - справка",
                        cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }, cancellationToken);
    }


    private async Task HandleCommandAsync(ITelegramBotClient botClient, Message message,
        CancellationToken cancellationToken)
    {
        var command = message.Text!.Split(' ')[0].ToLower();
        var chatId = message.Chat.Id;

        switch (command)
        {
            case "/start":
                await botClient.SendMessage(
                    chatId,
                    "🎵 Music Download Bot\n\n" +
                    "Просто отправь мне ссылку и я скачаю аудио в MP3!",
                    cancellationToken: cancellationToken);
                break;

            case "/help":
                await botClient.SendMessage(
                    chatId,
                    "📖 Как использовать:\n\n" +
                    "1. Скопируй ссылку\n" +
                    "2. Отправь ссылку мне\n" +
                    "3. Получи аудио файл!\n\n" +
                    "Пример: https://www.youtube.com/watch?v=...",
                    cancellationToken: cancellationToken);
                break;

            default:
                await botClient.SendMessage(
                    chatId,
                    "❌ Неизвестная команда. Используй /help для справки.",
                    cancellationToken: cancellationToken);
                break;
        }
    }

    private async Task HandleAudioDownloadAsync(
        ITelegramBotClient botClient,
        Message message,
        string url,
        CancellationToken cancellationToken)
    {
        var chatId = message.Chat.Id;

        var messageId = message.MessageId;

        try
        {
            var processingMessage = await botClient.SendMessage(
                chatId,
                "⏳ Начинаю загрузку аудио...",
                replyParameters: new ReplyParameters { MessageId = messageId },
                cancellationToken: cancellationToken);

            var downloadResult = await downloadStrategy.DownloadAudioAsync(url, cancellationToken);

            await using var stream = File.OpenRead(downloadResult.FilePath!);

            Stream? thumbStream = null;

            if (!string.IsNullOrWhiteSpace(downloadResult.ThumbnailUrl))
            {
                using var httpClient = new HttpClient();
                var bytes = await httpClient.GetByteArrayAsync(downloadResult.ThumbnailUrl, cancellationToken);
                thumbStream = new MemoryStream(bytes);
            }

            var displayName = !string.IsNullOrWhiteSpace(downloadResult.Title)
                ? downloadResult.Title
                : Path.GetFileNameWithoutExtension(downloadResult.FilePath);

            await botClient.SendAudio(
                chatId,
                InputFile.FromStream(stream, displayName),
                caption: displayName,
                thumbnail: thumbStream != null ? InputFile.FromStream(thumbStream, "thumb.jpg") : null,
                replyParameters: new ReplyParameters { MessageId = messageId },
                cancellationToken: cancellationToken);

            await botClient.DeleteMessage(chatId, processingMessage.MessageId, cancellationToken);

            if (File.Exists(downloadResult.FilePath))
                File.Delete(downloadResult.FilePath);
        }
        catch (Exception ex)
        {
            await botClient.SendMessage(
                chatId,
                $"❌ Ошибка: {ex.Message}",
                replyParameters: new ReplyParameters { MessageId = messageId },
                cancellationToken: cancellationToken);
        }
    }

    public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException =>
                $"Telegram API Error: {apiRequestException.ErrorCode} - {apiRequestException.Message}",
            _ => exception.Message
        };

        Console.WriteLine($"Bot error: {errorMessage}");
        return Task.CompletedTask;
    }

    private bool IsValidUrl(string text) => Uri.TryCreate(text, UriKind.Absolute, out _);
}