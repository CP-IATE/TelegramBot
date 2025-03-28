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
            if (message.Photo is { } photo)
            {
                await HandleFileAsync(photo[0].FileId, "Files\\Photo", cancellationToken);
            }
            else if (message.Document is { } document)
            {
                await HandleFileAsync(document.FileId, "Files\\Documents", cancellationToken);
            }
            else if (message.Video is { } video)
            {
                await HandleFileAsync(video.FileId, "Files\\Video", cancellationToken);
            }
            else if (message.Audio is { } audio)
            {
                await HandleFileAsync(audio.FileId, "Files\\Audio", cancellationToken);
            }
            else if (message.Voice is { } voice)
            {
                await HandleFileAsync(voice.FileId, "Files\\Voice", cancellationToken);
            }
            else if (message.Sticker is { } sticker)
            {
                await HandleFileAsync(sticker.FileId, "Files\\Sticker", cancellationToken);
            }
            else
            {
               await botClient.SendMessage(
                   message.Chat.Id, 
                   text: $"Message from {message.From.Id}\n{text}",
                   cancellationToken: cancellationToken
               ); 
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
    
    private async Task HandleFileAsync(string fileId, string directoryPath, CancellationToken cancellationToken)
    {
        var file = await _botClient.GetFile(fileId, cancellationToken);
        var filePath = Path.Combine(directoryPath, fileId + Path.GetExtension(file.FilePath));
        Directory.CreateDirectory(directoryPath);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await _botClient.DownloadFile(file.FilePath, fileStream, cancellationToken);
        }
    }
}