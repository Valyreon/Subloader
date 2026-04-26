using System.Threading.Tasks;

namespace SubloaderAvalonia.Interfaces;

public interface IGitHubService
{
    public Task<bool> IsLatestVersionAsync(string currentVersionTag);
}
