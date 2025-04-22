using Telegram.Bot;
using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Services.FileSenderStrategy;

public interface IFileSender
{
    Task SendFileAsync(List<AttachmentDto> attachments, long chatId, ITelegramBotClient botClient, string? caption = null);
}