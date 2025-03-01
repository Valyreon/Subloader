using OpenSubtitlesSharp.Services;

namespace OpenSubtitlesSharp.Tests.ServiceTests;

public class MovieHasherServiceTests
{
    private readonly MovieHasherService _movieHasherService;

    private static readonly byte[] randomByteArray =
    [
        0x01, 0x02, 0x03, 0x04, 0x05,
        0x06, 0x07, 0x08, 0x09, 0x0A,
        0x0B, 0x0C, 0x0D, 0x0E, 0x0F,
        0x10, 0x11, 0x12, 0x13, 0x14,
        0x15, 0x16, 0x17, 0x18, 0x19,
        0x1A, 0x1B, 0x1C, 0x1D, 0x1E,
        0x1F, 0x20, 0x21, 0x22, 0x23,
        0x24, 0x25, 0x26, 0x27, 0x28,
        0x29, 0x2A, 0x2B, 0x2C, 0x2D,
        0x2E, 0x2F, 0x30, 0x31, 0x32
    ];

    public MovieHasherServiceTests()
    {
        _movieHasherService = new MovieHasherService();
    }

    [Fact]
    public void Exists_WhenExists_CalculatesHash()
    {
        var file = "some_file_for_testing.mp4";

        File.WriteAllBytes(file, randomByteArray);

        _movieHasherService.ComputeMovieHash(file).ShouldBe("b1a39587796b6d90");

        File.Delete(file);
    }

    [Fact]
    public void Exists_WhenDoesntExist_ThrowsArgumentException()
    {
        var file = "some_file_for_testing.mp4";
        var resultTask = Task.Run(() => _movieHasherService.ComputeMovieHash(file));
        var exc = resultTask.ShouldThrowAsync<ArgumentException>();
    }
}
