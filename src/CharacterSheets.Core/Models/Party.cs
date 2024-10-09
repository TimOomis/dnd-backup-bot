namespace CharacterSheets.Core.Models;

public sealed record Party(string Name, IReadOnlyCollection<PartyMember> PartyMembers);
