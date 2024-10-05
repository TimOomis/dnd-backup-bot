using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.UseCases.Interfaces;

internal interface IBackupCharacterSheetUseCase
{
    Task Execute(IReadOnlyCollection<CharacterSheet> characterSheets);
}
