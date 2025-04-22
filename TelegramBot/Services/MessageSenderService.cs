using Telegram.Bot;
using Telegram.Bot.Types.Enums;
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
        if (message.message.attachments.Any())
        {
            if (message.message.attachments.Count > 1)
            {
                var fileSender = FileSenderFactory.GetFileSender("mediaGroup");
                await fileSender.SendFileAsync(message.message.attachments, chatId, _botClient, $"*Name:* `{message.author.name}({message.author.tag})`\n*Channel:* `{message.channel}`\n{message.message.text}");
            }
            else
            {
                var fileSender = FileSenderFactory.GetFileSender(message.message.attachments.First().type.Split('.').Last());
                await fileSender.SendFileAsync(message.message.attachments, chatId, _botClient, $"*Name:* `{message.author.name}({message.author.tag})`\n*Channel:* `{message.channel}`\n{message.message.text}");
            }
        }
        else
        {
            await _botClient.SendMessage(chatId, $"*Name:* `{message.author.name}({message.author.tag})`\n*Channel:* `{message.channel}`\n{message.message.text}", ParseMode.MarkdownV2);
        }
    }
}