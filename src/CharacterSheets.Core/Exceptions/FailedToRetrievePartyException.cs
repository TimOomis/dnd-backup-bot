namespace CharacterSheets.Core.Exceptions;

public class FailedToRetrievePartyException(Exception innerException)
    : Exception("Failed to retrieve the party.", innerException)
{
}
