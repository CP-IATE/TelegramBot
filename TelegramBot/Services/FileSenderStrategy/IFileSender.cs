using Telegram.Bot;
using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Services.FileSenderStrategy;

public interface IFileSender
{
    Task SendFileAsync(AttachmentDto attachment, long chatId, ITelegramBotClient botClient, string? caption = null);
}