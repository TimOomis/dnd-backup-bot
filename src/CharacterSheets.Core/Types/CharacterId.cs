namespace CharacterSheets.Core.Types;

public record struct CharacterId(ulong Value)
{
    public static implicit operator CharacterId(ulong Id)
    {
        return new(Id);
    }

    public static implicit operator ulong(CharacterId Id)
    {
        return Id.Value;
    }
}
