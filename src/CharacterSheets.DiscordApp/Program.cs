// See https://aka.ms/new-console-template for more information
using System.Reflection;
using CharacterSheets.Adapters.Discord.Configuration;
using CharacterSheets.Adapters.Discord.Extensions;
using CharacterSheets.Adapters.DndBeyond.Configuration;
using CharacterSheets.Adapters.DndBeyond.Extensions;
using CharacterSheets.Adapters.Files.Configuration;
using CharacterSheets.Adapters.Files.Extensions;
using CharacterSheets.Core.Extensions;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace CharacterSheets.DiscordApp;

public static class Program
{
    private static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "development";
        var logger = CreateLogger();

        Log.Logger = logger;

        return Host
            .CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder
                    .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path for appsettings.json
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // Add appsettings.json
                    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)  // Add appsettings.{environment}.json
                    .AddEnvironmentVariables()  // Optionally, load environment variables
                    .Build();
            })
            .ConfigureServices((context, services) =>
            {
                var dndBeyondSettings = context.Configuration.GetRequiredSection("DndBeyond").Get<DndBeyondSettings>()!;
                var fileSettings = context.Configuration.GetRequiredSection("Files").Get<FileSystemSettings>()!;
                var discordSettings = context.Configuration.GetRequiredSection("Discord").Get<DiscordSettings>()!;

                services
                    .AddCoreServices()
                    .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger))
                    .AddFilesAdapter(fileSettings)
                    .AddDndBeyondAdapter(dndBeyondSettings)
                    .AddDiscordAdapter(discordSettings)
                    .BuildServiceProvider();

                services.AddHostedService<Worker>();
            });
    }

    private static Serilog.Core.Logger CreateLogger()
    {
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();

        return logger;
    }
}
