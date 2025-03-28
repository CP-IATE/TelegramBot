using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TelegramBot.Services;

namespace TelegramBot;

public class Startup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddRouting(options => options.LowercaseUrls = true);
        
        services.AddServices();
        services.AddControllers();
        
        services.AddEndpointsApiExplorer()
            .AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1",
                new()
                {
                    Title = "Telegram Bot API",
                    Version = "v1"
                }
            );
            
            var filePath = Path.Combine(System.AppContext.BaseDirectory, "TelegramBotAPI.xml");
            c.IncludeXmlComments(filePath);
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                // policy.WithOrigins("http://localhost:5173")
                policy.SetIsOriginAllowed(origin => true)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithExposedHeaders("Access-Control-Allow-Origin");
            });
        });
    }

    public void Configure(IApplicationBuilder builder, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            builder.UseSwagger();
            builder.UseSwaggerUI();
        }

        builder.UseHttpsRedirection();

        builder.UseCors();
        
        builder.UseRouting();
        builder.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller}/{action}/{id?}");
        });
    }
}