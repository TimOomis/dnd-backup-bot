using CharacterSheets.Adapters.DndBeyond.Configuration;
using CharacterSheets.Adapters.DndBeyond.Ports;
using CharacterSheets.Core.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CharacterSheets.Adapters.DndBeyond.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDndBeyondAdapter(this IServiceCollection services, DndBeyondSettings settings)
    {
        services.TryAddSingleton(settings);

        services
            .AddHttpClient<ICharacterSheetStore, CharacterSheetStore>(client =>
            {
                client.BaseAddress = new Uri(settings.BaseAddress);
                client.DefaultRequestHeaders.Add("User-Agent", settings.UserAgent);
            })
            // TIL that as of NET8, Microsoft.Extensions.Http.Resilience is a thing and that it has a decent amount of built-in resiliency features.
            // I initially had some settings defined for setting this, but it basically covers them all! 🎉
            // See https://learn.microsoft.com/en-us/dotnet/core/resilience/http-resilience?tabs=dotnet-cli#standard-resilience-handler-defaults
            .AddStandardResilienceHandler();

        return services;
    }
}
