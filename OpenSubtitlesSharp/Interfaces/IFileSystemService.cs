using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace OpenSubtitlesSharp.Interfaces;

internal interface IFileSystemService
{
    public bool Exists(string path);
    public FileStream OpenRead(string path);
}
