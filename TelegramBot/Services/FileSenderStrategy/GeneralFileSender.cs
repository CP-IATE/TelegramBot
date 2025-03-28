using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Services.FileSenderStrategy;

public class GeneralFileSender : IFileSender
{
    public async Task SendFileAsync(AttachmentDto attachment, long chatId, ITelegramBotClient botClient, string? caption = null)
    {
        using (var fileStream = new MemoryStream(Convert.FromBase64String(attachment.data)))
        {
            await botClient.SendDocument(chatId, new InputFileStream(fileStream, $"{Guid.NewGuid()}.{attachment.type}"), caption: caption);
        }
    }
}