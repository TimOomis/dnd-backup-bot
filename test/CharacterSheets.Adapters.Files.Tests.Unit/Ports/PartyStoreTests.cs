using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using CharacterSheets.Adapters.Files.Configuration;
using CharacterSheets.Adapters.Files.Ports;
using CharacterSheets.Core.Exceptions;
using CharacterSheets.Core.Models;
using Serilog;

namespace CharacterSheets.Adapters.Files.Tests.Unit.Ports;

public class PartyStoreTests
{
    private readonly PartyStore _sut;
    private readonly MockFileSystem _fileSystem = new();
    private readonly FileSystemSettings _settings = new("file.json");
    private readonly Mock<ILogger> _logger = new(MockBehavior.Loose);

    public PartyStoreTests()
    {
        _sut = new PartyStore(_fileSystem, _settings, _logger.Object);
    }

    [Theory, AutoData]
    public async Task GivenAValidParty_WhenGettingParty_ThenReturnsExpectedParty(Party party)
    {
        // Arrange
        var json = JsonSerializer.Serialize(party);
        _fileSystem.AddFile(_settings.Path, new MockFileData(json));

        // Act
        var result = await _sut.GetParty();

        // Assert
        result.Should().BeEquivalentTo(party);
    }

    [Theory, AutoData]
    public async Task GivenAValidParty_WhenGettingParty_ThenLogsInformation(Party party)
    {
        // Arrange
        var json = JsonSerializer.Serialize(party);
        _fileSystem.AddFile(_settings.Path, new MockFileData(json));

        // Act
        await _sut.GetParty();

        // Assert
        _logger.Verify(x => x.Debug("Successfully deserialized the party file."), Times.Once);
    }

    [Fact]
    public async Task GivenAFileIsNotAvailable_WhenGettingParty_ThenThrowsFileNotFoundException()
    {
        // Act
        Func<Task> act = _sut.GetParty;

        // Assert
        (await act.Should().ThrowAsync<FailedToRetrievePartyException>())
            .Which.InnerException.Should().BeOfType<FileNotFoundException>();
    }

    [Fact]
    public async Task GivenAFileIsNotAvailable_WhenGettingParty_ThenLogsFileCouldNotBeFound()
    {
        // Act
        Func<Task> act = _sut.GetParty;

        // Assert
        await act.Should().ThrowAsync<FailedToRetrievePartyException>();

        _logger.Verify(
            x => x.Error(
                It.IsAny<FileNotFoundException>(),
                "Could not find the requested file at {Path}, please ensure the file exists.",
                _settings.Path),
            Times.Once);
    }

    [Fact]
    public async Task GivenAFileIsAvailable_WhenContainsInvalidFormat_ThenThrowsFailedToRetrievePartyException()
    {
        // Arrange
        _fileSystem.AddFile(_settings.Path, new MockFileData("invalid"));

        // Act
        Func<Task> act = _sut.GetParty;

        // Assert
        (await act.Should().ThrowAsync<FailedToRetrievePartyException>())
            .Which.InnerException.Should().BeOfType<JsonException>();
    }

    [Fact]
    public async Task GivenAFileIsAvailable_WhenContainsInvalidFormat_ThenLogsFailedToRetrieveParty()
    {
        // Arrange
        _fileSystem.AddFile(_settings.Path, new MockFileData("invalid"));

        // Act
        Func<Task> act = _sut.GetParty;

        // Assert
        await act.Should().ThrowAsync<FailedToRetrievePartyException>();

        _logger.Verify(
            x => x.Error(
                It.IsAny<JsonException>(),
                "Failed to deserialize the party file."),
            Times.Once);
    }

    [Theory, AutoData]
    public async Task GivenAFileIsAvailable_WhenFileSystemThrowsException_ThenThrowsFailedToRetrievePartyException(Exception exception)
    {
        // Arrange
        var fileSystemMock = new Mock<IFileSystem>();
        var file = new Mock<IFile>();

        file.Setup(f => f.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception);
        fileSystemMock.Setup(f => f.File).Returns(file.Object);

        var sut = new PartyStore(fileSystemMock.Object, _settings, _logger.Object);

        // Act
        Func<Task> act = sut.GetParty;

        // Assert
        (await act.Should().ThrowAsync<FailedToRetrievePartyException>())
            .Which.InnerException.Should().Be(exception);
    }

    // Given a file is available, when the file system throws an exception, verify the message is properly logged.

    [Theory, AutoData]
    public async Task GivenAFileIsAvailable_WhenFileSystemThrowsException_ThenLogsFailedToRetrieveParty(Exception exception)
    {
        // Arrange
        var fileSystemMock = new Mock<IFileSystem>();
        var file = new Mock<IFile>();

        file.Setup(f => f.ReadAllTextAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ThrowsAsync(exception);
        fileSystemMock.Setup(f => f.File).Returns(file.Object);

        var sut = new PartyStore(fileSystemMock.Object, _settings, _logger.Object);

        // Act
        Func<Task> act = sut.GetParty;

        // Assert
        await act.Should().ThrowAsync<FailedToRetrievePartyException>();

        _logger.Verify(
            x => x.Error(
                exception,
                "Failed to retrieve the party due to an unexpected error."),
            Times.Once);
    }
}
