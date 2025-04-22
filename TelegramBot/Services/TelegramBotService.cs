using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Persistence.DTOs;
using TelegramBot.Persistence.Requests;

namespace TelegramBot.Services;

public sealed class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;
    private readonly HttpClientService _httpClientService;
    
    private readonly Dictionary<string, List<AttachmentDto>> _mediaGroupBuffer = new();
    private readonly Dictionary<string, DateTime> _lastReceivedTimes = new();
    private readonly Dictionary<string, Message> _mediaGroupMessages = new();

    public TelegramBotService(ITelegramBotClient botClient, ReceiverOptions receiverOptions, HttpClientService httpClientService)
    {
        _botClient = botClient;
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            DropPendingUpdates = true
        };
        _httpClientService = httpClientService;
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
            
            var attachments = new List<AttachmentDto>();
            string? mediaGroupId = message.MediaGroupId;
            
            if (message.Photo != null)
            {
                attachments.Add(await SaveFileAsync(message.Photo.Last().FileId, "Files\\Photos", cancellationToken));
            }
            if (message.Document != null)
            {
                attachments.Add(await SaveFileAsync(message.Document.FileId, "Files\\Documents", cancellationToken));
            }
            if (message.Video != null)
            {
                attachments.Add(await SaveFileAsync(message.Video.FileId, "Files\\Video", cancellationToken));
            }
            if (message.Audio != null)
            {
                attachments.Add(await SaveFileAsync(message.Audio.FileId, "Files\\Audio", cancellationToken));
            }
            if (message.Voice != null)
            {
                attachments.Add(await SaveFileAsync(message.Voice.FileId, "Files\\Voice", cancellationToken));
            }
            if (message.Sticker != null)
            {
                attachments.Add(await SaveFileAsync(message.Sticker.FileId, "Files\\Sticker", cancellationToken));
            }
            
            if (!string.IsNullOrEmpty(mediaGroupId))
            {
                if (!_mediaGroupBuffer.ContainsKey(mediaGroupId))
                    _mediaGroupBuffer[mediaGroupId] = new List<AttachmentDto>();

                _mediaGroupBuffer[mediaGroupId].AddRange(attachments);
                if (!_mediaGroupMessages.ContainsKey(mediaGroupId))
                {
                    _mediaGroupMessages[mediaGroupId] = message;
                }
                _lastReceivedTimes[mediaGroupId] = DateTime.Now;

                DebounceMediaGroupAsync(mediaGroupId, async () =>
                {
                    if (_mediaGroupBuffer.TryGetValue(mediaGroupId, out var bufferedAttachments))
                    {
                        _mediaGroupMessages.TryGetValue(mediaGroupId, out var groupMessage);
                        await _httpClientService.PostAsync(await FormSendMessageRequest(groupMessage ?? message, bufferedAttachments));
                        _mediaGroupBuffer.Remove(mediaGroupId);
                        _lastReceivedTimes.Remove(mediaGroupId);
                        _mediaGroupMessages.Remove(mediaGroupId);
                    }
                }, 2000);
            }
            else
            {
                await _httpClientService.PostAsync(await FormSendMessageRequest(message, attachments));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void DebounceMediaGroupAsync(string groupId, Func<Task> action, int delayMilliseconds)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(delayMilliseconds);
                if (_lastReceivedTimes.ContainsKey(groupId) &&
                    (DateTime.Now - _lastReceivedTimes[groupId]).TotalMilliseconds >= delayMilliseconds)
                {
                    await action();
                }
            }
            catch (TaskCanceledException)
            {
                // ignored - debounce reset
            }
        });
    }
    
    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
    
private async Task<AttachmentDto> SaveFileAsync(string fileId, string directoryPath, CancellationToken cancellationToken)
{
    var file = await _botClient.GetFile(fileId, cancellationToken);
    var filePath = Path.Combine(directoryPath, fileId + Path.GetExtension(file.FilePath));
    Directory.CreateDirectory(directoryPath);

    await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
    {
        await _botClient.DownloadFile(file.FilePath!, fileStream, cancellationToken);
    }

    byte[] fileBytes = await File.ReadAllBytesAsync(filePath, cancellationToken);
    string base64EncodedFile = Convert.ToBase64String(fileBytes);

    File.Delete(filePath);

    return new AttachmentDto
    {
        type = file.FilePath.Split('/').Last(),
        data = base64EncodedFile
    };
}
    
    private Task<SendMessageRequest> FormSendMessageRequest(Message message, List<AttachmentDto> attachments)
    {
        if (attachments.Any())
        {
            return Task.FromResult(new SendMessageRequest
            {
                platform = "telegram",
                channel = message.Chat.Title,
                author = new MessageAuthorDto
                {
                    tag = message.From.Username ?? message.From.Id.ToString(),
                    name = message.From.FirstName
                },
                message = new MessageDto
                {
                    text = message.Caption ?? "",
                    attachments = attachments
                }
            });
        }
        return Task.FromResult(new SendMessageRequest
        {
            platform = "telegram",
            channel = message.Chat.Title,
            author = new MessageAuthorDto
            {
                tag = message.From.Username ?? message.From.Id.ToString(),
                name = message.From.FirstName
            },
            message = new MessageDto
            {
                text = message.Text ?? ""
            }
        });
    }
}