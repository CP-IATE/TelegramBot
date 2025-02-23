using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegramBot.Services;

namespace TelegramBot;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddServices();
        services.AddControllers();
    }

    public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        builder.UseRouting();
        builder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}