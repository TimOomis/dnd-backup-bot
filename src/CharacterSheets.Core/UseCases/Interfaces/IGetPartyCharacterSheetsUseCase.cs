using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.UseCases.Interfaces;

public interface IGetPartyCharacterSheetsUseCase
{
    Task<IReadOnlyCollection<CharacterSheet>> Execute();
}
