using OpenSubtitlesSharp;
using SubloaderAvalonia.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubloaderAvalonia.Interfaces;

public interface IOpenSubtitlesService
{
    Task<DownloadInfo> DownloadSubtitleAsync(SubtitleEntry subtitle, string videoPath, string savePath = null);

    Task<IEnumerable<string>> GetFormatsAsync();

    Task<IEnumerable<SubtitleLanguage>> GetLanguagesAsync();

    Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> GetSubtitlesForFileAsync(string filePath, int currentPage = 1);

    Task<User> LoginAsync(string username, string password);

    Task<bool> LogoutAsync();

    Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> SearchSubtitlesAsync(
        string token,
        int? episodeNumber = null,
        int? seasonNumber = null,
        int? year = null,
        FileTypeFilter? filter = null,
        int? imdbId = null,
        int? parentImdbId = null,
        int currentPage = 0);
}
