using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace OpenSubtitlesSharp.Interfaces;

internal interface IFileSystemService
{
    bool Exists(string path);
    FileStream OpenRead(string path);
}
