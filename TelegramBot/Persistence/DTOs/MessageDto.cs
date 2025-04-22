using Telegram.Bot.Types;

namespace TelegramBot.Persistence.DTOs;

public class MessageDto
{
    public string? text { get; set; }
    public List<AttachmentDto> attachments { get; set; } = [];
}