using Microsoft.Extensions.Logging;

namespace CharacterSheets.Core.Tests.Shared.Extensions;
public static class LoggerAssertions
{
    public static void VerifyError<T>(this Mock<ILogger<T>> mockedLogger, Func<string, bool> predicate, Func<Times> times) =>
        mockedLogger.VerifyLogMessage(LogLevel.Error, predicate, times);

    public static void VerifyInformation<T>(this Mock<ILogger<T>> mockedLogger, Func<string, bool> predicate, Func<Times> times) =>
        mockedLogger.VerifyLogMessage(LogLevel.Information, predicate, times);

    public static void VerifyDebug<T>(this Mock<ILogger<T>> mockedLogger, Func<string, bool> predicate, Func<Times> times) =>
        mockedLogger.VerifyLogMessage(LogLevel.Debug, predicate, times);

    public static void VerifyWarning<T>(this Mock<ILogger<T>> mockedLogger, Func<string, bool> predicate, Func<Times> times) =>
        mockedLogger.VerifyLogMessage(LogLevel.Warning, predicate, times);

    private static void VerifyLogMessage<T>(this Mock<ILogger<T>> mockedLogger, LogLevel logLevel, Func<string, bool> predicate, Func<Times> times)
    {
        mockedLogger.Verify(x => x.Log(logLevel, 0, It.Is<object>(p => predicate(p.ToString() ?? "")), null, It.IsAny<Func<object, Exception?, string>>()), times);
    }

}
