﻿using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using Microsoft.Extensions.Logging;
using Serilog;

namespace CharacterSheets.Adapters.DndBeyond.Ports;

internal class CharacterSheetStore(HttpClient httpClient, ILogger<CharacterSheetStore> logger) : ICharacterSheetStore
{
    public async Task<CharacterSheet> GetSheet(PartyMember partyMember)
    {
        try
        {
            var bytes = await httpClient.GetByteArrayAsync(GetFileName(partyMember));

            logger.LogDebug("Successfully downloaded the character sheet for {CharacterName}.", partyMember.CharacterName);

            return new CharacterSheet(
                FileName: $"{partyMember.CharacterName}.pdf",
                Data: bytes,
                CharacterName: partyMember.CharacterName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to download the character sheet for {CharacterName}.", partyMember.CharacterName);
            throw new FailedToRetrieveCharacterSheetException(ex);
        }
    }

    private static string GetFileName(PartyMember member) => $"{member.AccountName}_{member.CharacterId.Value}.pdf";
}
