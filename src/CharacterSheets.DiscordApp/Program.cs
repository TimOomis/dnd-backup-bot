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
using Serilog;
using Serilog.Events;

namespace CharacterSheets.DiscordApp;

public static class Program
{
    private static async Task Main(string[] args)
    {
        var configuration = GetConfiguration();
        var logger = CreateLogger();

        Log.Logger = logger;

        var dndBeyondSettings = configuration.GetRequiredSection("DndBeyond").Get<DndBeyondSettings>()!;
        var fileSettings = configuration.GetRequiredSection("Files").Get<FileSystemSettings>()!;
        var discordSettings = configuration.GetRequiredSection("Discord").Get<DiscordSettings>()!;
        var services = new ServiceCollection();

        var serviceProvider = services
            .AddCoreServices()
            .AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger))
            .AddFilesAdapter(fileSettings)
            .AddDndBeyondAdapter(dndBeyondSettings)
            .AddDiscordAdapter(discordSettings)
            .BuildServiceProvider();

        await RunBotAsync(serviceProvider, discordSettings);
    }

    private static async Task RunBotAsync(IServiceProvider serviceProvider, DiscordSettings discordSettings)
    {
        var client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        var interactionService = serviceProvider.GetRequiredService<InteractionService>();

        client.Log += LogAsync;
        interactionService.Log += LogAsync;

        // Usually AddModulesAsync would work, but it doesn't for some reason. Maybe a NET8 support thing.
        // Given that manual registraiton works, we'll go with that for now.
        await interactionService.RegisterCommands(serviceProvider);

        client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(client, x);
            await interactionService.ExecuteCommandAsync(ctx, serviceProvider);
        };

        client.Ready += async () =>
        {
            await interactionService.RegisterCommandsToGuildAsync(discordSettings.GuildId, true);
            await interactionService.AddModulesToGuildAsync(discordSettings.GuildId, true);
            await interactionService.RegisterCommandsGloballyAsync(true);
        };

        await client.LoginAsync(TokenType.Bot, discordSettings.Token);
        await client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }

    private static async Task LogAsync(LogMessage message)
    {
        var severity = message.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };
        Log.Write(severity, message.Exception, "[{Source}] {Message}", message.Source, message.Message);
        await Task.CompletedTask;
    }

    private static IConfigurationRoot GetConfiguration()
    {
        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())  // Set the base path for appsettings.json
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)  // Add appsettings.json
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)  // Add appsettings.{environment}.json
            .AddEnvironmentVariables()  // Optionally, load environment variables
            .Build();
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
