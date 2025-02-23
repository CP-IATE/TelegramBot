using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

public sealed class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ReceiverOptions _receiverOptions;

    public TelegramBotService(ITelegramBotClient botClient, ReceiverOptions receiverOptions)
    {
        _botClient = botClient;
        _receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = new[] { UpdateType.Message },
            DropPendingUpdates = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _botClient.StartReceiving(UpdateHandler, ErrorHandler, _receiverOptions, stoppingToken);

        var me = await _botClient.GetMe(stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        try
        {
            if (update.Message is not { } message || message.Text is not { } text) return;
            Console.WriteLine($"Message from {message.From.Id}\nText: {text}");
            await botClient.SendMessage(
                message.Chat.Id,
                text,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private Task ErrorHandler(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
        return Task.CompletedTask;
    }
}