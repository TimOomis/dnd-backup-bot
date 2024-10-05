using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using CharacterSheets.Core.UseCases.Interfaces;
using Serilog;

namespace CharacterSheets.Core.UseCases;

internal class GetPartyCharacterSheetsUseCase(
    IPartyStore partyStore,
    ICharacterSheetStore characterSheetStore,
    ILogger logger) : IGetPartyCharacterSheetsUseCase
{
    public async Task<IReadOnlyCollection<CharacterSheet>> Execute()
    {
        try
        {
            return await ExecuteInternal();
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Failed to retrieve character sheets due to an unexpected error");
            throw new CouldNotGetPartyCharacterSheetsException(ex);
        }
    }

    private async Task<IReadOnlyCollection<CharacterSheet>> ExecuteInternal()
    {
        var party = await partyStore.GetParty();

        if (party.PartyMembers.Count == 0)
        {
            logger.Warning("No party members found");
            throw new ArgumentException("The party cannot be empty.");
        }

        var sheets = (await Task.WhenAll(party.PartyMembers.Select(GetSheet))).ToList();

        logger.Information("Successfully retrieved {SheetCount} character sheets.", sheets.Count);
        return sheets;
    }

    private Task<CharacterSheet> GetSheet(PartyMember partyMember) => characterSheetStore.GetSheet(partyMember);
}
