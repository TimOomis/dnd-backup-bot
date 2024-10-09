using System;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using CharacterSheets.Core.Tests.Shared.Attributes;
using CharacterSheets.Core.UseCases;
using Microsoft.Extensions.Logging;

namespace CharacterSheets.Core.Tests.Unit.UseCases;

public class GetPartyCharacterSheetsUseCaseTests
{
    private readonly Mock<ICharacterSheetStore> _characterSheetStoreMock = new(MockBehavior.Strict);
    private readonly Mock<IPartyStore> _partyStoreMock = new(MockBehavior.Strict);
    private readonly Mock<ILogger<GetPartyCharacterSheetsUseCase>> _loggerMock = new(MockBehavior.Loose);

    private readonly GetPartyCharacterSheetsUseCase _sut;

    public GetPartyCharacterSheetsUseCaseTests()
    {
        _sut = new(_partyStoreMock.Object, _characterSheetStoreMock.Object, _loggerMock.Object);
    }

    [Theory, StreamAutoData]
    public async Task GivenPartyAndSheetsAreAvailable_WhenGettingCharacterSheets_ThenReturnsExpectedSheets(Party party, CharacterSheet sheet)
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ReturnsAsync(party);

        _characterSheetStoreMock
            .Setup(csm => csm.GetSheet(It.IsAny<PartyMember>()))
            .ReturnsAsync(sheet);

        // Act
        var result = await _sut.Execute();

        // Arrange
        result.Count.Should().Be(party.PartyMembers.Count);
        result.Should().ContainEquivalentOf(sheet);
    }

    [Theory, StreamAutoData]
    public async Task GivenPartyAndSheetsAreAvailable_WhenGettingCharacterSheets_ThenShouldHaveCalledStoresExpectedAmounts(Party party, CharacterSheet sheet)
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ReturnsAsync(party);

        _characterSheetStoreMock
            .Setup(csm => csm.GetSheet(It.IsAny<PartyMember>()))
            .ReturnsAsync(sheet);

        // Act
        await _sut.Execute();

        // Arrange
        _partyStoreMock.Verify(psm => psm.GetParty(), Times.Once);
        _characterSheetStoreMock.Verify(csm => csm.GetSheet(It.Is<PartyMember>(p => party.PartyMembers.Contains(p))), Times.Exactly(party.PartyMembers.Count));
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), StreamAutoData]
    public async Task GivenPartyAndSheetsAreAvailable_WhenGettingCharacterSheets_ThenLogsCreatedSheets(Party party, CharacterSheet sheet)
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ReturnsAsync(party);

        _characterSheetStoreMock
            .Setup(csm => csm.GetSheet(It.IsAny<PartyMember>()))
            .ReturnsAsync(sheet);

        // Act
        await _sut.Execute();

        // Arrange
        _loggerMock.Verify(l => l.LogInformation("Successfully retrieved {SheetCount} character sheets.", party.PartyMembers.Count), Times.Once);
    }

    [Theory, AutoData]
    public async Task GivenPartyStoreThrowsException_WhenGettingCharactersSheets_ThenThrowsCouldNotGetPartyCharacterSheetsException(Exception exception)
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ThrowsAsync(exception);

        // Act
        var act = _sut.Execute;

        // Assert
        (await act.Should().ThrowExactlyAsync<CouldNotGetPartyCharacterSheetsException>())
            .Which.InnerException.Should().Be(exception);
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), AutoData]
    public async Task GivenPartyStoreThrowsException_WhenGettingCharactersSheets_ThenLogsUnexpectedExceptionWasThrown(Exception exception)
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ThrowsAsync(exception);

        // Act
        var act = _sut.Execute;

        // Assert
        await act.Should().ThrowExactlyAsync<CouldNotGetPartyCharacterSheetsException>();
        _loggerMock.Verify(lm => lm.LogError(exception, "Failed to retrieve character sheets due to an unexpected error"), Times.Once);
    }

    [Fact]
    public async Task GivenAPartyIsEmpty_WhenGettingCharactersSheets_ThenThrowsArgumentException()
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ReturnsAsync(new Party("test", []));

        // Act
        var act = _sut.Execute;

        // Assert
        await act.Should().ThrowExactlyAsync<ArgumentException>()
            .WithMessage("The party cannot be empty.");
    }

    [Fact(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify.")]
    public async Task GivenAPartyIsEmpty_WhenGettingCharactersSheets_ThenLogsThatPartyIsEmpty()
    {
        // Arrange
        _partyStoreMock
            .Setup(psm => psm.GetParty())
            .ReturnsAsync(new Party("test", []));

        // Act
        var act = _sut.Execute;

        // Assert
        await act.Should().ThrowExactlyAsync<ArgumentException>();

        _loggerMock.Verify(lm => lm.LogWarning("No party members found"), Times.Once);
    }
}
