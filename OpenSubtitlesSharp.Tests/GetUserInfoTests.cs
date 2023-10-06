using System.Net;
using System.Text.Json;
using OpenSubtitlesSharp.Dtos;

namespace OpenSubtitlesSharp.Tests;

public class GetUserInfoTests
{
    [Fact]
    public async Task GetUserInfo_WhenNoToken_ThrowsInvalidOperationException()
    {
        // ARRANGE
        var httpClient = new HttpClient(new Mock<HttpMessageHandler>(MockBehavior.Strict).Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var resultTask = osClient.GetUserInfoAsync();
        await resultTask.ShouldThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task GetUserInfoAsync_WhenSuccess_ReturnsCorrectData()
    {
        var userInfo = new UserInfo
        {
            AllowedDownloads = 10,
            DownloadsCount = 5,
            ExtInstalled = true,
            Level = "Premium",
            RemainingDownloads = 5,
            UserId = 12345,
            Vip = true
        };

        var userInfoResponse = new UserInfoResponse
        {
            Data = userInfo
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/infos/user")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(userInfoResponse)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key", "my-token");

        // ACT
        var result = await osClient.GetUserInfoAsync();
        result.ShouldBeEquivalentTo(userInfo);
    }

    [Fact]
    public async Task GetUserInfoAsync_WhenFail_ThrowsRequestFailedException()
    {
        var response = new ErrorResponse
        {
            Status = HttpStatusCode.InternalServerError
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/infos/user")),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.InternalServerError,
               Content = new StringContent(JsonSerializer.Serialize(response)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key", "my-token");

        // ACT
        var resultTask = osClient.GetUserInfoAsync();

        // ASSERT
        var exception = await resultTask.ShouldThrowAsync<RequestFailedException>();
        exception.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        exception.Message.ShouldBe("Something went wrong.");
    }
}
