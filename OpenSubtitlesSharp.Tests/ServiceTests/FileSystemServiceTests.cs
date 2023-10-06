using OpenSubtitlesSharp.Services;

namespace OpenSubtitlesSharp.Tests.ServiceTests;

public class FileSystemServiceTests
{
    private readonly FileSystemService _fileSystemService;

    public FileSystemServiceTests() {
        _fileSystemService = new FileSystemService();
    }

    [Fact]
    public void Exists_WhenExists_ReturnsTrue()
    {
        var file = "some_file_for_testing.txt";

        File.WriteAllText(file, "Some content");

        _fileSystemService.Exists(file).ShouldBeTrue();

        File.Delete(file);
    }

    [Fact]

    public void Exists_WhenDoesntExist_ReturnsFalse()
    {
        _fileSystemService.Exists(Guid.NewGuid().ToString() + ".txt").ShouldBeFalse();
    }

    [Fact]

    public void OpenRead_WhenExists_Success()
    {
        var file = "some_file_for_testing.txt";

        File.WriteAllText(file, "Some content");

        using var stream = File.OpenRead(file);

        stream.ShouldNotBeNull();
        stream.ReadByte();
    }

    [Fact]

    public async void OpenRead_WhenDoesntExist_Error()
    {
        var task = Task.Run(() => _fileSystemService.OpenRead(Guid.NewGuid().ToString() + ".txt"));
        await task.ShouldThrowAsync<FileNotFoundException>();
    }
}
