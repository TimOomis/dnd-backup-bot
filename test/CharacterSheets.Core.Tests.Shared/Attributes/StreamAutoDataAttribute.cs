using AutoFixture;
using CharacterSheets.Core.Tests.Shared.Customizations;

namespace CharacterSheets.Core.Tests.Shared.Attributes;

/// <summary>
/// Customization for AutoFixture to allow creation of Stream Fixtures. 
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class StreamAutoDataAttribute : AutoDataAttribute
{
    public StreamAutoDataAttribute() : base(GetFixture)
    {
    }

    private static Fixture GetFixture()
    {
        var fixture = new Fixture();

        fixture.Customize(new MemoryStreamCustomization());

        return fixture;
    }
}
