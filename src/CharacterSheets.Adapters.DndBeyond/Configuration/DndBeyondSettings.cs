namespace CharacterSheets.Adapters.DndBeyond.Configuration;

// TODO: Put in appsettings when we create the console app:
// Base Address: https://www.dndbeyond.com/sheet-pdfs/
// User Agent: CharacterSheets/1.0
public record DndBeyondSettings(string BaseAddress, string UserAgent);
