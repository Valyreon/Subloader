using OpenSubtitlesSharp;
using SubloaderAvalonia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubloaderAvalonia.Interfaces;

public interface IOpenSubtitlesService
{
    public Task<DownloadInfo> DownloadSubtitleAsync(SubtitleEntry subtitle, string videoPath, string savePath = null);

    public Task<IEnumerable<string>> GetFormatsAsync();

    public Task<IEnumerable<SubtitleLanguage>> GetLanguagesAsync();

    public Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> GetSubtitlesForFileAsync(string filePath, int currentPage = 1);

    public Task<User> LoginAsync(string username, string password);

    public Task<bool> LogoutAsync();

    public Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> SearchSubtitlesAsync(
        string token,
        int? episodeNumber = null,
        int? seasonNumber = null,
        int? year = null,
        FileTypeFilter? filter = null,
        int? imdbId = null,
        int? parentImdbId = null,
        int currentPage = 0);
}
