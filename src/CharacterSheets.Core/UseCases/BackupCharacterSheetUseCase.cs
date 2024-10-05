using CharacterSheets.Core.Configuration;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using CharacterSheets.Core.UseCases.Interfaces;
using Serilog;

namespace CharacterSheets.Core.UseCases;

public class BackupCharacterSheetUseCase(
    ICharacterSheetPublisher characterSheetPublisher,
    PublisherSettings publishSettings,
    ILogger logger) : IBackupCharacterSheetUseCase
{
    public async Task Execute(IReadOnlyCollection<CharacterSheet> characterSheets)
    {
        try
        {
            await ExecuteInternal(characterSheets);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to publish character sheets due to an unexpected error");
            throw new PublishingSheetsFailedException(ex);
        }
    }

    private async Task ExecuteInternal(IReadOnlyCollection<CharacterSheet> characterSheets)
    {
        await characterSheetPublisher.Publish(publishSettings.DisplayName, characterSheets);

        logger.Information("Character sheets have been published!");
    }
}
