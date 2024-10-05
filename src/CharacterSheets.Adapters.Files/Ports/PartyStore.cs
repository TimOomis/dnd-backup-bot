using System.IO.Abstractions;
using System.Text.Json;
using CharacterSheets.Adapters.Files.Configuration;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using Serilog;

namespace CharacterSheets.Adapters.Files.Ports;

internal class PartyStore(
    IFileSystem fileSystem,
    FileSystemSettings settings,
    ILogger logger) : IPartyStore
{
    public async Task<Party> GetParty()
    {
        try
        {
            var file = await fileSystem.File.ReadAllTextAsync(settings.Path) ?? string.Empty;
            var party = JsonSerializer.Deserialize<Party>(file) ?? throw new InvalidDataException("Expected deserialized party to not be null.");

            logger.Debug("Successfully deserialized the party file.");
            return party;
        }
        catch (FileNotFoundException ex)
        {
            logger.Error(ex, "Could not find the requested file at {Path}, please ensure the file exists.", settings.Path);
            throw new FailedToRetrievePartyException(ex);
        }
        catch (JsonException ex)
        {
            logger.Error(ex, "Failed to deserialize the party file.");
            throw new FailedToRetrievePartyException(ex);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to retrieve the party due to an unexpected error.");
            throw new FailedToRetrievePartyException(ex);
        }

    }
}
