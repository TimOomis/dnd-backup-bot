using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.UseCases.Interfaces;

public interface IBackupCharacterSheetUseCase
{
    Task Execute(IReadOnlyCollection<CharacterSheet> characterSheets);
}
