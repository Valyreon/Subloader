using System.Net;
using System.Text.Json;
using OpenSubtitlesSharp.Dtos;

namespace OpenSubtitlesSharp.Tests;

public class SearchTests
{
    [Fact]
    public async Task SearchAsync_WhenSuccess_ReturnsSubtitlesInfo()
    {
        var searchResult = new SearchResult
        {
            Items =
            [
                new Subtitle
                {
                    Id = "123",
                    Type = "Subtitle",
                    Information = new SubtitleInformation
                    {
                        AiTranslated = true,
                        Comments = "This is a great subtitle!",
                        DownloadCount = 1000,
                        FeatureDetails = new FeatureDetails
                        {
                            EpisodeNumber = 1,
                            FeatureId = 1234,
                            FeatureType = "Movie",
                            ImdbId = 5678,
                            MovieName = "Sample Movie",
                            ParentFeatureId = null,
                            ParentImdbId = null,
                            ParentTitle = null,
                            ParentTmdbId = null,
                            SeasonNumber = null,
                            Title = "Sample Movie",
                            TmdbId = 9876,
                            Year = 2023
                        },
                        Files =
                        [
                            new SubtitleFileInfo
                            {
                                CdNumber = 1,
                                FileId = 567,
                                FileName = "subtitle.srt"
                            }
                        ],
                        ForeignPartsOnly = false,
                        Fps = 24.0,
                        FromTrusted = true,
                        Hd = true,
                        HearingImpaired = false,
                        Language = "English",
                        LegacySubtitleId = 7890,
                        MachineTranslated = false,
                        NewDownloadCount = 200,
                        Ratings = 4.5,
                        RelatedLinks =
                        [
                            new RelatedLink
                            {
                                ImgUrl = "https://example.com/image.jpg",
                                Label = "Website",
                                Url = "https://example.com"
                            }
                        ],
                        Release = "Sample Release",
                        SubtitleId = "12345",
                        UploadDate = DateTime.Now,
                        Uploader = new Uploader
                        {
                            Name = "John Doe",
                            Rank = "Gold",
                            UploaderId = 5678
                        },
                        Url = "https://example.com/subtitle",
                        Votes = 500
                    }
                }
            ],
            Page = 1,
            PerPage = 10,
            TotalCount = 1,
            TotalPages = 1
        };

        const string requestLink = "https://api.opensubtitles.com/api/v1/subtitles?moviehash=abc123&query=Sample+Movie&type=movie";

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri(requestLink)),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.OK,
               Content = new StringContent(JsonSerializer.Serialize(searchResult)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        var searchParams = new SearchParameters
        {
            MovieHash = "abc123",
            Type = FileTypeFilter.Movie,
            Query = "Sample Movie"
        };

        // ACT
        var result = await osClient.SearchAsync(searchParams);
        result.ShouldBeEquivalentTo(searchResult);
    }

    [Fact]
    public async Task SearchAsync_WhenFail_ThrowsRequestFailedException()
    {
        var searchResult = new ErrorResponse
        {
            Message = "Something went wrong",
            Status = HttpStatusCode.NotAcceptable
        };

        const string requestLink = "https://api.opensubtitles.com/api/v1/subtitles?moviehash=abc123&query=Sample+Movie&type=movie";

        // ARRANGE
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
              "SendAsync",
              ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get
                                            && req.RequestUri == new Uri(requestLink)),
              ItExpr.IsAny<CancellationToken>()
           )
           .ReturnsAsync(new HttpResponseMessage()
           {
               StatusCode = HttpStatusCode.NotAcceptable,
               Content = new StringContent(JsonSerializer.Serialize(searchResult)),
           })
           .Verifiable();

        var httpClient = new HttpClient(handlerMock.Object);

        var osClient = new OpenSubtitlesClient(httpClient, null, null, "my-api-key");

        var searchParams = new SearchParameters
        {
            MovieHash = "abc123",
            Type = FileTypeFilter.Movie,
            Query = "Sample Movie"
        };

        // ACT
        var task = osClient.SearchAsync(searchParams);

        // ASSERT
        var exc = await task.ShouldThrowAsync<RequestFailedException>();
        exc.Message.ShouldBe("Something went wrong");
        exc.StatusCode.ShouldBe(HttpStatusCode.NotAcceptable);
    }
}
