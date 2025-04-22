using System.Text.Json.Serialization;

namespace TelegramBot.Persistence.DTOs;

public class AttachmentDto
{
    public string type { get; init; }
    public string data { get; init; }
}