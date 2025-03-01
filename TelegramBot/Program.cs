﻿using DotNetEnv;
using Microsoft.AspNetCore.Builder;

namespace TelegramBot;

public static class Program
{
    private static void Main(string[] args)
    {
        Env.Load();

        var builder = WebApplication.CreateBuilder(args);

        var startup = new Startup();

        startup.ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();
        startup.Configure(app, app.Environment);
        app.MapControllers();
        app.Run();
    }
}