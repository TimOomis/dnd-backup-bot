using CharacterSheets.Core.Types;

namespace CharacterSheets.Core.Models;

public sealed record PartyMember(string CharacterName, string AccountName, CharacterId CharacterId);
