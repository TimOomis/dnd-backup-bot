using System.IO.Abstractions;
using System.Text.Json;
using CharacterSheets.Adapters.Files.Configuration;
using CharacterSheets.Adapters.Files.Models;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using Microsoft.Extensions.Logging;

namespace CharacterSheets.Adapters.Files.Ports;

internal class PartyStore(
    IFileSystem fileSystem,
    FileSystemSettings settings,
    ILogger<PartyStore> logger) : IPartyStore
{
    public async Task<Party> GetParty()
    {
        try
        {
            var file = await fileSystem.File.ReadAllTextAsync(settings.PartyJsonFilePath) ?? string.Empty;
            var party = JsonSerializer.Deserialize<PartyDto>(file) ?? throw new InvalidDataException("Expected deserialized party to not be null.");

            logger.LogDebug("Successfully deserialized the party file.");
            return party.ToDomain();
        }
        catch (FileNotFoundException ex)
        {
            logger.LogError(ex, "Could not find the requested file at {Path}, please ensure the file exists.", settings.PartyJsonFilePath);
            throw new FailedToRetrievePartyException(ex);
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "Failed to deserialize the party file.");
            throw new FailedToRetrievePartyException(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve the party due to an unexpected error.");
            throw new FailedToRetrievePartyException(ex);
        }

    }
}
