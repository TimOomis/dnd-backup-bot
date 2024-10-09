using CharacterSheets.Adapters.Discord.Configuration;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog;
using CharacterSheets.Adapters.Discord.Extensions;
using System;

namespace CharacterSheets.DiscordApp;

internal class Worker(
    DiscordSocketClient client,
    InteractionService interactionService,
    DiscordSettings discordSettings,
    IServiceProvider serviceProvider,
    ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetupBotAsync();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunBotAsync();
                // Doing some tasks
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "An error occurred when running the bot");
            }
        }
    }

    private async Task SetupBotAsync()
    {
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
    }

    private async Task RunBotAsync()
    {
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
}
