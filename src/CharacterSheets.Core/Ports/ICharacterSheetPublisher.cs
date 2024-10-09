using CharacterSheets.Core.Models;

namespace CharacterSheets.Core.Ports;

public interface ICharacterSheetPublisher
{
    public Task Publish(string displayName, IReadOnlyCollection<CharacterSheet> characterSheets);
}
