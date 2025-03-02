using System.Net;
using System.Text.Json;

namespace OpenSubtitlesSharp.Tests;

public class GetLanguagesTests
{
    [Fact]
    public async Task GetLanguagesAsync_WhenSuccess_ReturnsCorrectData()
    {
        var languagesResponse = new LanguagesResponse
        {
            Data =
            [
                new SubtitleLanguage
                {
                    Code = "en",
                    Name = "English"
                },
                new SubtitleLanguage
                {
                    Code = "fr",
                    Name = "French"
                },
                new SubtitleLanguage
                {
                    Code = "es",
                    Name = "Spanish"
                }
            ]
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/infos/languages")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(languagesResponse)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var result = await osClient.GetLanguagesAsync();

        result.ShouldBeEquivalentTo(languagesResponse.Data);
    }

    [Fact]
    public async Task GetLanguagesAsync_WhenFail_ThrowsRequestFailedException()
    {
        var errorResponse = new MessageResponse
        {
            Status = HttpStatusCode.BadRequest,
            Message = "Something happened"
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/infos/languages")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.BadRequest,
               Content = new StringContent(JsonSerializer.Serialize(errorResponse)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var resultTask = osClient.GetLanguagesAsync();
        var exception = await resultTask.ShouldThrowAsync<RequestFailedException>();

        exception.Message.ShouldBe("Something happened");
        exception.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
