namespace CharacterSheets.Core.Types;

public record struct CharacterId(int Value)
{
    public static implicit operator CharacterId(int Id)
    {
        return new(Id);
    }

    public static implicit operator int(CharacterId Id)
    {
        return Id.Value;
    }
}
