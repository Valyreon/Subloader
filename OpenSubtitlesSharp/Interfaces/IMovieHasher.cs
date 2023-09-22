using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace OpenSubtitlesSharp.Interfaces;

internal interface IMovieHasherService
{
    string ComputeMovieHash(string filename);
}
