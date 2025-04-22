using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Services.FileSenderStrategy;

public class VideoFileSender : IFileSender
{
    public async Task SendFileAsync(List<AttachmentDto> attachments, long chatId, ITelegramBotClient botClient, string? caption = null)
    {
        var attachment = attachments.First();
        using (var fileStream = new MemoryStream(Convert.FromBase64String(attachment.data)))
        {
            await botClient.SendVideo(chatId, new InputFileStream(fileStream, $"{Guid.NewGuid()}.{attachment.type}"), caption: caption, ParseMode.MarkdownV2);
        }
    }
}