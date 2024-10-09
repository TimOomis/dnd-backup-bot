using CharacterSheets.Core.Models;

namespace CharacterSheets.Adapters.Files.Models;

public record PartyMemberDto(string CharacterName, string AccountName, ulong CharacterId)
{
    public PartyMember ToDomain() => new(CharacterName, AccountName, new(CharacterId));
}
