using TelegramBot.Persistence.DTOs;

namespace TelegramBot.Persistence.Requests;

public class SendMessageRequest
{
    public string platform { get; init; }
    public string channel { get; init; }
    public MessageAuthorDto author { get; init; }
    public MessageDto message { get; init; }
}