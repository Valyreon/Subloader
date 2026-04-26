using System.Threading.Tasks;

namespace SubloaderWpf.Interfaces;
public interface IGitHubService
{
    public Task<bool> IsLatestVersionAsync(string currentVersionTag);
}
