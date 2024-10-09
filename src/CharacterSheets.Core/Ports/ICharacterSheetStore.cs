using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.Ports;

public interface ICharacterSheetStore
{
    Task<CharacterSheet> GetSheet(PartyMember partyMember);
}
