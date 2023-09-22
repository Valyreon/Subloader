using System.Net;
using System.Text.Json;

namespace OpenSubtitlesSharp.Tests;

public class LoginTests
{
    [Fact]
    public async Task LoginAsync_WhenSuccess_ReturnsInfo()
    {
        var restResult = new LoginInfo
        {
            BaseUrl = "vip-api.opensubtitles.com",
            Status = HttpStatusCode.OK,
            Token = "the-jwt-token",
            User = new UserInfo
            {
                DownloadsCount = 5,
                RemainingDownloads = 95,
                AllowedDownloads = 100,
                ExtInstalled = false,
                Level = "Leecher",
                UserId = 475,
                Vip = true
            }
        };

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/login")),
              ItExpr.IsAny<CancellationToken>()
           )
           .Callback<HttpRequestMessage, CancellationToken>(async (req, token) =>
           {
               var contentString = req.Content != null ? await req.Content.ReadAsStringAsync() : string.Empty;
               contentString.ShouldBe("{\"username\":\"my-username\",\"password\":\"my-password\"}");
           })
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(restResult)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var result = await osClient.LoginAsync("my-username", "my-password");
        result.ShouldBeEquivalentTo(restResult);
        osClient.Token.ShouldBe("the-jwt-token");
    }

    [Fact]
    public async Task LoginAsync_WhenUnauthorized_TrimsMessage_ThrowsRequestFailedException()
    {
        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/login")),
              ItExpr.IsAny<CancellationToken>()
           )
           .Callback<HttpRequestMessage, CancellationToken>(async (req, token) =>
           {
               // You can add your custom logic here
               var contentString = req.Content != null ? await req.Content.ReadAsStringAsync() : string.Empty;
               contentString.ShouldBe("{\"username\":\"my-username\",\"password\":\"my-password\"}");
           })
           // prepare the expected response of the mocked http call
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.Unauthorized,
               Content = new StringContent(JsonSerializer.Serialize(new MessageResponse
               {
                   Message = "Error, invalid username/password   ",
                   Status = HttpStatusCode.Unauthorized
               })),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var task = osClient.LoginAsync("my-username", "my-password");

        // ASSERT
        var exc = await task.ShouldThrowAsync<RequestFailedException>();
        exc.Message.ShouldBe("Error, invalid username/password");
        exc.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
