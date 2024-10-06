using CharacterSheets.Core.UseCases;
using CharacterSheets.Core.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterSheets.Core.Extensions;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services) => services.AddCoreUseCases();

    private static IServiceCollection AddCoreUseCases(this IServiceCollection services)
    {
        services.AddSingleton<IGetPartyCharacterSheetsUseCase, GetPartyCharacterSheetsUseCase>();
        services.AddSingleton<IBackupCharacterSheetUseCase, BackupCharacterSheetUseCase>();

        return services;
    }
}
