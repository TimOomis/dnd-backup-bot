using CharacterSheets.Core.Types;

namespace CharacterSheets.Core.Models;

public sealed record PartyMember(string Name, string AccountName, CharacterId CharacterId);
