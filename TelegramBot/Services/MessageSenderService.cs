using Telegram.Bot;
using TelegramBot.Persistence.Requests;
using TelegramBot.Services.FileSenderStrategy;

namespace TelegramBot.Services;

public class MessageSenderService
{
    private readonly ITelegramBotClient _botClient;
    
    public MessageSenderService(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }
    
    public async Task SendMessageAsync(long chatId, SendMessageRequest message)
    {
        if (message.message.attachments is { } attachments)
        {
            foreach (var attachment in attachments)
            {
                var fileSender = FileSenderFactory.GetFileSender(attachment.type);
                await fileSender.SendFileAsync(attachment, chatId, _botClient, $"{message.author.tag}\n{message.author.name}\nFrom {message.channel}\n{message.message.text}");
            }
        }
        else
        {
            await _botClient.SendMessage(chatId, $"{message.author.tag}\n{message.author.name}\nFrom {message.channel}\n{message.message.text}");
        }
    }
}