using OpenSubtitlesSharp;
using SubloaderWpf.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubloaderWpf.Interfaces;

public interface IOpenSubtitlesService
{
    Task<DownloadInfo> DownloadSubtitleAsync(SubtitleEntry subtitle, string videoPath, string savePath = null);

    Task<IEnumerable<string>> GetFormatsAsync();

    Task<IEnumerable<SubtitleLanguage>> GetLanguagesAsync();

    Task<IEnumerable<SubtitleEntry>> GetSubtitlesForFileAsync(string filePath);

    Task<User> LoginAsync(string username, string password);

    Task<bool> LogoutAsync();

    Task<IEnumerable<SubtitleEntry>> SearchSubtitlesAsync(
        string token,
        int? episodeNumber = null,
        int? seasonNumber = null,
        int? year = null,
        FileTypeFilter? filter = null,
        int? imdbId = null,
        int? parentImdbId = null);
}
