using Telegram.Bot;

namespace TelegramBot.Services;

public class MessageSenderService
{
    private readonly ITelegramBotClient _botClient;
    
    public MessageSenderService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task SendTextMessage(long chatId, string message, CancellationToken cancellationToken)
    {
        await _botClient.SendMessage(chatId, message, cancellationToken: cancellationToken);
    }
}