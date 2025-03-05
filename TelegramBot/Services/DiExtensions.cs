using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace TelegramBot.Services;

public static class DiExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection collection) =>
        collection
            .AddSingleton<ITelegramBotClient>(new TelegramBotClient(Environment.GetEnvironmentVariable("BOT_TOKEN")))
            .AddSingleton<ReceiverOptions>()
            .AddHostedService<TelegramBotService>()
            .AddScoped<MessageSenderService>();
}