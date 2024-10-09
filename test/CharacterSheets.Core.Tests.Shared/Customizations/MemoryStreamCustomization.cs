using AutoFixture;

namespace CharacterSheets.Core.Tests.Shared.Customizations;

public class MemoryStreamCustomization : ICustomization
{
    public void Customize(IFixture fixture) => fixture.Register<Stream>(() => new MemoryStream());
}
