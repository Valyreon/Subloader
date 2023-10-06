using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using SubloaderAvalonia.Interfaces;

namespace SubloaderAvalonia.Services;

public class GitHubService : IGitHubService
{
    private static readonly Uri latestReleaseUri = new("https://api.github.com/repos/Valyreon/Subloader/releases/latest");

    public async Task<bool> IsLatestVersionAsync(string currentVersionTag)
    {
        using var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(5),
        };

        httpClient.DefaultRequestHeaders.Add("User-Agent", "Subloader " + App.VersionTag);

        var result = await httpClient.GetFromJsonAsync<JsonDocument>(latestReleaseUri);
        var latestTag = result.RootElement.GetProperty("tag_name").GetString();
        return string.CompareOrdinal(latestTag, currentVersionTag) <= 0;
    }
}
