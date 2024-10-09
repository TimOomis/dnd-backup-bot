using CharacterSheets.Adapters.Discord.Commands;
using CharacterSheets.Adapters.Discord.Configuration;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CharacterSheets.Adapters.Discord.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscordAdapter(this IServiceCollection services, DiscordSettings settings)
    {
        services.TryAddSingleton(settings);

        var config = new DiscordSocketConfig
        {
            LogGatewayIntentWarnings = false,
            LogLevel = LogSeverity.Debug,
            GatewayIntents = GatewayIntents.AllUnprivileged, 
        };

        // X represents either Interaction or Command, as it functions the exact same for both types.
        var interactionServiceConfig = new InteractionServiceConfig()
        {
            LogLevel = LogSeverity.Debug,
            DefaultRunMode = RunMode.Async
            //...
        };

        return services
            .AddCommands()
            .AddSingleton(config)
            .AddSingleton<DiscordSocketClient>()
            .AddSingleton(interactionServiceConfig)
            .AddSingleton(sp => new InteractionService(sp.GetRequiredService<DiscordSocketClient>(), sp.GetRequiredService<InteractionServiceConfig>()));
    }

    private static IServiceCollection AddCommands(this IServiceCollection services) => services.AddSingleton<BackupCommand>();

    public static async Task RegisterCommands(this InteractionService interactionService, IServiceProvider serviceProvider) => await interactionService.AddModuleAsync<BackupCommand>(serviceProvider);
}
