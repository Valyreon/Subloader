using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.Tests;

public class ParseFileNameForSearchParametersTests
{
    private readonly Mock<IFileSystemService> _fileSystemServiceMock = new();
    private readonly Mock<IMovieHasherService> _movieHasherServiceMock = new();

    [Fact]
    public void ParseFileName_WhenIncludeEpisodeData_ParametersCorrect()
    {
        const string fileName = "Sometime.In.2020.S02E05.mp4";

        _fileSystemServiceMock.Setup(s => s.Exists(fileName)).Returns(true);
        _movieHasherServiceMock.Setup(r => r.ComputeMovieHash(fileName)).Returns("a1b2c3");

        var osClient = new OpenSubtitlesClient(new HttpClient(), _fileSystemServiceMock.Object, _movieHasherServiceMock.Object, "api-key");

        var parameters = osClient.ParseFileNameForSearchParameters(fileName);

        parameters.EpisodeNumber.ShouldBe(5);
        parameters.SeasonNumber.ShouldBe(2);
        parameters.Query.ShouldBe("Sometime In 2020");
        parameters.Year.ShouldBeNull();
        parameters.MovieHash.ShouldBe("a1b2c3");
        parameters.Type.ShouldBe(FileTypeFilter.Episode);
    }

    [Fact]
    public void ParseFileName_WhenIncludeYear_ParametersCorrect()
    {
        const string fileName = "Sometime.In.2020.(2012).mp4";

        _fileSystemServiceMock.Setup(s => s.Exists(fileName)).Returns(true);
        _movieHasherServiceMock.Setup(r => r.ComputeMovieHash(fileName)).Returns("a1b2c3");

        var osClient = new OpenSubtitlesClient(new HttpClient(), _fileSystemServiceMock.Object, _movieHasherServiceMock.Object, "api-key");

        var parameters = osClient.ParseFileNameForSearchParameters(fileName);

        parameters.EpisodeNumber.ShouldBe(null);
        parameters.SeasonNumber.ShouldBe(null);
        parameters.Query.ShouldBe("Sometime In 2020");
        parameters.Year.ShouldBe(2012);
        parameters.MovieHash.ShouldBe("a1b2c3");
        parameters.Type.ShouldNotBe(FileTypeFilter.Episode);
        parameters.Type.ShouldNotBe(FileTypeFilter.Movie);
    }
}
