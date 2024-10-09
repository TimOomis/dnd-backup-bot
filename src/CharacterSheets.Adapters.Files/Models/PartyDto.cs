using CharacterSheets.Core.Models;

namespace CharacterSheets.Adapters.Files.Models;

public record PartyDto(string Name, IReadOnlyCollection<PartyMemberDto> Members)
{
    public Party ToDomain() => new(Name, Members.Select(m => m.ToDomain()).ToList());
}
