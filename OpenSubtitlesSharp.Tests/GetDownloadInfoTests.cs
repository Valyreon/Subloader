using System.Net;
using System.Text.Json;
using OpenSubtitlesSharp.Dtos;

namespace OpenSubtitlesSharp.Tests;

public class GetDownloadInfoTests
{
    [Fact]
    public async Task GetDownloadInfoAsync_WhenSuccess_ReturnsCorrectData()
    {
        var downloadInfo = new DownloadInfo
        {
            FileName = "sample_subtitle.srt",
            Link = "https://example.com/downloads/sample_subtitle.srt",
            Message = "Download in progress",
            Remaining = 5,
            Requests = 10,
            ResetTime = "2023-09-22T12:00:00",
            ResetTimeUtc = DateTime.Parse("2023-09-22T12:00:00Z")
        };

        // ARRANGE
        var contentString = string.Empty;
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/download")),
              ItExpr.IsAny<CancellationToken>()
           )
           .Callback<HttpRequestMessage, CancellationToken>(
            async (req, token) => contentString = req.Content != null ? await req.Content.ReadAsStringAsync(token) : string.Empty)
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(downloadInfo)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var result = await osClient.GetDownloadInfoAsync(new DownloadParameters
        {
            FileId = 1234,
            FileName = Path.GetFileName(downloadInfo.FileName),
            ForceDownload = false,
            InFPS = 25,
            OutFPS = 30,
            SubFormat = "srt",
            Timeshift = 2.5M,
        });

        contentString.ShouldBe("{\"file_id\":1234,\"file_name\":\"sample_subtitle.srt\",\"force_download\":false,\"in_fps\":25,\"out_fps\":30,\"sub_format\":\"srt\",\"timeshift\":2.5}");
        result.ShouldBeEquivalentTo(downloadInfo);
    }

    [Fact]
    public async Task GetDownloadInfoAsync_WithFileId_WhenSuccess_ReturnsCorrectData()
    {
        var downloadInfo = new DownloadInfo
        {
            FileName = "sample_subtitle.srt",
            Link = "https://example.com/downloads/sample_subtitle.srt",
            Message = "Download in progress",
            Remaining = 5,
            Requests = 10,
            ResetTime = "2023-09-22T12:00:00",
            ResetTimeUtc = DateTime.Parse("2023-09-22T12:00:00Z")
        };

        // ARRANGE
        var contentString = string.Empty;
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/download")),
              ItExpr.IsAny<CancellationToken>()
           )
           .Callback<HttpRequestMessage, CancellationToken>(async (req, token) => contentString = req.Content != null ? await req.Content.ReadAsStringAsync(token) : string.Empty)
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(downloadInfo)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var result = await osClient.GetDownloadInfoAsync(1234);

        contentString.ShouldBe("{\"file_id\":1234}");
        result.ShouldBeEquivalentTo(downloadInfo);
    }

    [Fact]
    public async Task GetDownloadInfoAsync_WithFileId_WhenFailed_ThrowsRequestFailedException()
    {
        var response = new ErrorResponse
        {
            Status = HttpStatusCode.InternalServerError,
            Errors = ["Something went wrong with our API"]
        };

        // ARRANGE
        var contentString = string.Empty;
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Post
                                            && req.RequestUri == new Uri("https://api.opensubtitles.com/api/v1/download")),
              ItExpr.IsAny<CancellationToken>()
           )
           .Callback<HttpRequestMessage, CancellationToken>(async (req, token) => contentString = req.Content != null ? await req.Content.ReadAsStringAsync(token) : string.Empty)
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.InternalServerError,
               Content = new StringContent(JsonSerializer.Serialize(response)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        // ACT
        var resultTask = osClient.GetDownloadInfoAsync(1234);

        var exception = await resultTask.ShouldThrowAsync<RequestFailedException>();
        exception.StatusCode.ShouldBe(HttpStatusCode.InternalServerError);
        exception.Message.ShouldBe("Something went wrong with our API");
    }
}
