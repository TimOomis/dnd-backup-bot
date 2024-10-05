using System.IO.Abstractions;
using CharacterSheets.Adapters.Files.Configuration;
using CharacterSheets.Adapters.Files.Ports;
using CharacterSheets.Core.Ports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CharacterSheets.Adapters.Files.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFilesAdapter(this IServiceCollection services, FileSystemSettings settings)
    {
        services.TryAddSingleton(settings);
        services.TryAddSingleton<IFileSystem, FileSystem>();

        services.AddSingleton<IPartyStore, PartyStore>();

        return services;
    }
}
