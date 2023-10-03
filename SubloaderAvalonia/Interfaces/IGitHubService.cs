using System.Threading.Tasks;

namespace SubloaderAvalonia.Interfaces;

public interface IGitHubService
{
    Task<bool> IsLatestVersionAsync(string currentVersionTag);
}
