using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.Services;

namespace TelegramBot.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    
    public TestController(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }
    
    /// <summary>
    /// <para>Test endpoint</para>
    /// Use this endpoint to test if the server is running
    /// </summary>
    [HttpGet("test")]
    [ProducesResponseType(typeof(bool), 200)]
    public IActionResult Test()
    {
        return Ok(true);
    }
    
    /// <summary>
    /// <para>Test message sending</para>
    /// Use this endpoint to test if the bot can send text messages
    [HttpPost("sendmessage")]
    [ProducesDefaultResponseType]
    public async Task<IActionResult> SendMessage([FromQuery] long chatId, string message)
    {
        var context = _serviceProvider.GetRequiredService<MessageSenderService>();
        await context.SendTextMessage(chatId, message, CancellationToken.None);
        return Ok();
    }
}