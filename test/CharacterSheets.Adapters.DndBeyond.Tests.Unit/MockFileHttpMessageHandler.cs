using System.Net;

namespace CharacterSheets.Adapters.DndBeyond.Tests.Unit;

public class MockFileHttpMessageHandler(byte[] data, HttpStatusCode statusCode = HttpStatusCode.OK) : HttpMessageHandler
{
    protected sealed override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage()
        {
            StatusCode = statusCode,
            Content = new ByteArrayContent(data),
        };

        return Task.FromResult(response);
    }
}
