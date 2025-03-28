using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

public sealed class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;

    public TelegramBotService(ITelegramBotClient botClient, ReceiverOptions receiverOptions)
    {
        _botClient = botClient;
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            DropPendingUpdates = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, cancellationToken);

        var me = await _botClient.GetMe(cancellationToken);

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not { } message) return;
            var text = message.Text;
            Console.WriteLine($"Message from {message.From.Id}\n {text}");

            switch (message)
            {
                case { Photo: { } photo }:
                    await SaveFileAsync(photo.Last().FileId, "Files\\Photo", cancellationToken);
                    break;
                case { Document: { } document }:
                    await SaveFileAsync(document.FileId, "Files\\Documents", cancellationToken);
                    break;
                case { Video: { } video }:
                    await SaveFileAsync(video.FileId, "Files\\Video", cancellationToken);
                    break;
                case { Audio: { } audio }:
                    await SaveFileAsync(audio.FileId, "Files\\Audio", cancellationToken);
                    break;
                case { Voice: { } voice }:
                    await SaveFileAsync(voice.FileId, "Files\\Voice", cancellationToken);
                    break;
                case { Sticker: { } sticker }:
                    await SaveFileAsync(sticker.FileId, "Files\\Sticker", cancellationToken);
                    break;
                default:
                    await botClient.SendMessage(
                        message.Chat.Id,
                        text: $"Message from {message.From.Id}\n{text}",
                        cancellationToken: cancellationToken
                    );
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
    
    private async Task SaveFileAsync(string fileId, string directoryPath, CancellationToken cancellationToken)
    {
        var file = await _botClient.GetFile(fileId, cancellationToken);
        var filePath = Path.Combine(directoryPath, fileId + Path.GetExtension(file.FilePath));
        Directory.CreateDirectory(directoryPath);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await _botClient.DownloadFile(file.FilePath!, fileStream, cancellationToken);
        }
    }
}