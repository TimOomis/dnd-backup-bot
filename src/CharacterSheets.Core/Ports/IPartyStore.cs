using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.Ports;

public interface IPartyStore
{
    // Made with the assumption we're only reading a static file with a single party.
    Task<Party> GetParty();
}
