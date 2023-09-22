using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.Services;

internal class FileSystemService : IFileSystemService
{
    public bool Exists(string path)
    {
        return File.Exists(path);
    }

    public FileStream OpenRead(string path)
    {
        return File.OpenRead(path);
    }
}
