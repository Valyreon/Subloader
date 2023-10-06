using System.Threading.Tasks;

namespace SubloaderWpf.Interfaces;
public interface IGitHubService
{
    Task<bool> IsLatestVersionAsync(string currentVersionTag);
}
