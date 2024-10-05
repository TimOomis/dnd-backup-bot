using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.UseCases.Interfaces;

internal interface IGetPartyCharacterSheetsUseCase
{
    Task<IReadOnlyCollection<CharacterSheet>> Execute();
}
