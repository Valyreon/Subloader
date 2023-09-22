using System.Net;
using System.Text.Json;

namespace OpenSubtitlesSharp.Tests;

public class LogoutTests
{
    [Fact]
    public async Task LogoutAsync_WhenSuccess_ReturnsTrue()
    {
        var restResult = new MessageResponse
        {
            Message = "token successfully destroyed",
            Status = HttpStatusCode.OK
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/logout")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(restResult)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key", "my-token");

        // ACT
        var result = await osClient.LogoutAsync();
        result.ShouldBeTrue();
    }

    [Fact]
    public async Task LogoutAsync_WhenNoToken_ThrowsInvalidOperationException()
    {
        // ARRANGE
        var httpClient = new HttpClient(new Mock<HttpMessageHandler>(MockBehavior.Strict).Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var resultTask = osClient.LogoutAsync();
        await resultTask.ShouldThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task LogoutAsync_WhenFail_ThrowsRequestFailedException()
    {
        var restResult = new MessageResponse
        {
            Message = "something failed",
            Status = HttpStatusCode.NotAcceptable
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Delete
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/logout")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.NotAcceptable,
               Content = new StringContent(JsonSerializer.Serialize(restResult)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key", "my-token");

        // ACT
        var resultTask = osClient.LogoutAsync();
        var exception = await resultTask.ShouldThrowAsync<RequestFailedException>();
        exception.Message.ShouldBe("something failed");
        exception.StatusCode.ShouldBe(HttpStatusCode.NotAcceptable);
    }
}
