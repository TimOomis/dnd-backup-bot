namespace CharacterSheets.Core.Exceptions;

public class PublishingSheetsFailedException(Exception innerException)
    : Exception("Failed to publish character sheets.", innerException)
{
}
