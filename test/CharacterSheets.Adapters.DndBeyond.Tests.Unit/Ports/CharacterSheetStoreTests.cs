using System.Net;
using CharacterSheets.Adapters.DndBeyond.Ports;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Tests.Shared.Attributes;
using CharacterSheets.Core.Tests.Shared.Extensions;
using Microsoft.Extensions.Logging;
using Serilog.Events;

namespace CharacterSheets.Adapters.DndBeyond.Tests.Unit.Ports;

public class CharacterSheetStoreTests
{
    private readonly Mock<ILogger<CharacterSheetStore>> _logger = new(MockBehavior.Loose);

    [Theory, StreamAutoData]
    public async Task GivenTheFileIsAvailable_WhenGettingSheet_ThenReturnsExpectedSheet(PartyMember partyMember, byte[] binaryData)
    {
        // Arrange
        var mockMessageHandler = new MockFileHttpMessageHandler(binaryData, HttpStatusCode.OK);
        var sut = CreateSut(mockMessageHandler);

        // Act
        var result = await sut.GetSheet(partyMember);

        // Assert
        result.FileName.Should().Be($"{partyMember.CharacterName}.pdf");
        result.Data.Should().BeEquivalentTo(binaryData);
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), StreamAutoData]
    public async Task GivenTheFileIsAvailable_WhenGettingSheet_ThenLogsFileDownloadInformation(PartyMember partyMember, byte[] binaryData)
    {
        // Arrange
        var mockMessageHandler = new MockFileHttpMessageHandler(binaryData, HttpStatusCode.OK);
        var sut = CreateSut(mockMessageHandler);

        // Act
        await sut.GetSheet(partyMember);

        // Assert
        _logger.Verify(l => l.LogDebug("Successfully downloaded the character sheet for {CharacterName}.", partyMember.CharacterName), Times.Once);
    }

    [Theory, StreamAutoData]
    public async Task GivenTheFileIsAvailable_WhenGettingSheet_ThenThrowsFailedToRetrieveCharacterSheetException(PartyMember partyMember)
    {
        // Arrange
        var mockMessageHandler = new MockFileHttpMessageHandler([], HttpStatusCode.NotFound);
        var sut = CreateSut(mockMessageHandler);

        // Act
        var act = () => sut.GetSheet(partyMember);

        // Assert
        (await act.Should().ThrowExactlyAsync<FailedToRetrieveCharacterSheetException>())
            .Which.InnerException.Should().BeOfType<HttpRequestException>();
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), StreamAutoData]
    public async Task GivenTheFileIsAvailable_WhenGettingSheet_ThenLogsErrorInformation(PartyMember partyMember)
    {
        // Arrange
        var mockMessageHandler = new MockFileHttpMessageHandler([], HttpStatusCode.NotFound);
        var sut = CreateSut(mockMessageHandler);

        var list = new List<string>();


        _logger
            .Setup(c => c.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception?, string>>()))
            .Callback((IInvocation invocation) =>
            {
                list.Add(invocation.ToString()!);
            })
            .Verifiable();

        // Act
        Func<Task> act = () => sut.GetSheet(partyMember);

        // Assert
        await act.Should().ThrowExactlyAsync<FailedToRetrieveCharacterSheetException>();

        _logger.Verify(l => l.LogError(It.IsAny<HttpRequestException>(), "Failed to download the character sheet for {CharacterName}.", partyMember.CharacterName), Times.Once);
    }

    private CharacterSheetStore CreateSut(HttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("http://localhost")
        };

        return new(httpClient, _logger.Object);
    }
}
