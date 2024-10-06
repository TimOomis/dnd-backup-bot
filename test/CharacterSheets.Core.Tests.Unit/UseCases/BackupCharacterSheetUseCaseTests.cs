using CharacterSheets.Core.Configuration;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using CharacterSheets.Core.Ports;
using CharacterSheets.Core.Tests.Shared.Attributes;
using CharacterSheets.Core.UseCases;
using Microsoft.Extensions.Logging;

namespace CharacterSheets.Core.Tests.Unit.UseCases;
public class BackupCharacterSheetUseCaseTests
{
    private readonly BackupCharacterSheetUseCase _sut;
    private readonly Mock<ICharacterSheetPublisher> _characterSheetPublisherMock = new(MockBehavior.Strict);
    private readonly Mock<ILogger<BackupCharacterSheetUseCase>> _logger = new(MockBehavior.Loose);
    private readonly PublisherSettings _settings = new(DisplayName: "Publish'er? I hardly know 'er!");

    public BackupCharacterSheetUseCaseTests()
    {
        _sut = new BackupCharacterSheetUseCase(_characterSheetPublisherMock.Object, _settings, _logger.Object);
    }

    [Theory, StreamAutoData]
    public async Task GivenSheetsArePublished_WhenPublished_ThenPublisherShouldBeCalledOnce(CharacterSheet[] sheets)
    {
        // Arrange
        _characterSheetPublisherMock
            .Setup(p => p.Publish(_settings.DisplayName, sheets))
            .Returns(Task.CompletedTask);

        // Act 
        await _sut.Execute(sheets);
    
        // Assert
        _characterSheetPublisherMock.Verify(p => p.Publish(_settings.DisplayName, sheets), Times.Once);
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), StreamAutoData]
    public async Task GivenSheetsArePublished_WhenPublished_ThenLoggerShouldHaveLoggedPublishing(CharacterSheet[] sheets)
    {
        // Arrange
        _characterSheetPublisherMock
            .Setup(p => p.Publish(_settings.DisplayName, sheets))
            .Returns(Task.CompletedTask);

        // Act
        await _sut.Execute(sheets);

        // Assert
        _logger.Verify(l => l.LogInformation("Character sheets have been published!"), Times.Once);
    }

    [Theory, StreamAutoData]
    public async Task GivenPublisherThrowsException_WhenPublished_ThenThrowsPublishingSheetsFailedException(CharacterSheet[] sheets, Exception exception)
    {
        // Arrange
        _characterSheetPublisherMock
            .Setup(p => p.Publish(_settings.DisplayName, sheets))
            .ThrowsAsync(exception);

        // Act
        var act = () => _sut.Execute(sheets);

        // Assert
        (await act.Should().ThrowExactlyAsync<PublishingSheetsFailedException>())
            .Which
            .InnerException.Should().Be(exception);
    }

    [Theory(Skip = "Pending changes to allow Microsoft.Extensions.Logging to properly verify."), StreamAutoData]
    public async Task GivenPublisherThrowsException_WhenPublished_ThenLogsUnexpectedExceptionOccurred(CharacterSheet[] sheets, Exception exception)
    {
        // Arrange
        _characterSheetPublisherMock
            .Setup(p => p.Publish(_settings.DisplayName, sheets))
            .ThrowsAsync(exception);

        // Act
        var act = () => _sut.Execute(sheets);

        // Assert
        await act.Should().ThrowExactlyAsync<PublishingSheetsFailedException>();
        _logger.Verify(l => l.LogError(exception, "Failed to publish character sheets due to an unexpected error"), Times.Once);
    }
}
