using System.Text;
using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.Services;

public class MovieHasherService : IMovieHasherService
{
    private readonly IFileSystemService _fileSystemService;

    public MovieHasherService()
    {
        _fileSystemService = new FileSystemService();
    }

    internal MovieHasherService(IFileSystemService fileSystemService)
    {
        _fileSystemService = fileSystemService;
    }

    public string ComputeMovieHash(string filename)
    {
        if (!_fileSystemService.Exists(filename))
        {
            throw new ArgumentException("File doesn't exist.", nameof(filename));
        }

        return ToHexadecimal(ComputeMovieHashToByte(filename));
    }

    private static byte[] ComputeMovieHash(Stream input)
    {
        long lhash, streamsize;
        streamsize = input.Length;
        lhash = streamsize;

        long i = 0;
        var buffer = new byte[sizeof(long)];
        while (i < 65536 / sizeof(long) && input.Read(buffer, 0, sizeof(long)) > 0)
        {
            i++;
            lhash += BitConverter.ToInt64(buffer, 0);
        }

        input.Position = Math.Max(0, streamsize - 65536);
        i = 0;
        while (i < 65536 / sizeof(long) && input.Read(buffer, 0, sizeof(long)) > 0)
        {
            i++;
            lhash += BitConverter.ToInt64(buffer, 0);
        }

        input.Close();
        var result = BitConverter.GetBytes(lhash);
        Array.Reverse(result);
        return result;
    }

    private byte[] ComputeMovieHashToByte(string filename)
    {
        using var input = _fileSystemService.OpenRead(filename);
        return ComputeMovieHash(input);
    }

    private static string ToHexadecimal(byte[] bytes)
    {
        var hexBuilder = new StringBuilder();
        for (var i = 0; i < bytes.Length; i++)
        {
            hexBuilder.Append(bytes[i].ToString("x2"));
        }

        return hexBuilder.ToString();
    }
}
