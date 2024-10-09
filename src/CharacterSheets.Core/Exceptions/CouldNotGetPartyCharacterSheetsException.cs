namespace CharacterSheets.Core.Exceptions;
public class CouldNotGetPartyCharacterSheetsException(Exception innerException)
    : Exception("Could not get the party's character sheets.", innerException)
{
}
