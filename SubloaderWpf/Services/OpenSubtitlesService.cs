using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fastenshtein;
using OpenSubtitlesSharp;
using SubloaderWpf.Extensions;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.Services;

public class OpenSubtitlesService(Lazy<ApplicationSettings> settings) : IOpenSubtitlesService
{
    private readonly Lazy<ApplicationSettings> _settings = settings;

    public async Task<DownloadInfo> DownloadSubtitleAsync(SubtitleEntry subtitle, string videoPath, string savePath = null)
    {
        using var osClient = GetClient();

        var downloadParameters = new DownloadParameters
        {
            FileId = subtitle.FileId,
            SubFormat = _settings.Value.PreferredFormat
        };

        var downloadInfo = await osClient.GetDownloadInfoAsync(downloadParameters);
        var extension = Path.GetExtension(downloadInfo.FileName);

        var destination = string.IsNullOrWhiteSpace(savePath)
            ? GetDestinationPath(videoPath, subtitle.LanguageCode, extension)
            : savePath;

        File.WriteAllBytes(destination, await GetRawFileAsync(downloadInfo.Link));

        return downloadInfo;
    }

    public async Task<IEnumerable<string>> GetFormatsAsync()
    {
        using var newClient = GetClient();
        return await newClient.GetSubtitleFormatsAsync();
    }

    public async Task<IEnumerable<SubtitleLanguage>> GetLanguagesAsync()
    {
        using var newClient = GetClient();
        return await newClient.GetLanguagesAsync();
    }

    public async Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> GetSubtitlesForFileAsync(string filePath, int currentPage = 1)
    {
        using var newClient = GetClient(_settings.Value.ForceDefaultApiUrl);

        var parameters = _settings.Value.DefaultSearchParameters with
        {
            Languages = _settings.Value.WantedLanguages,
            Page = currentPage
        };

        var result = await newClient.SearchAsync(filePath, parameters);

        // order by levenshtein distance
        var wantedLanguages = StaticResources.AllLanguages.Where(l => _settings.Value.WantedLanguages.Contains(l.Code));
        var laven = new Levenshtein(Path.GetFileNameWithoutExtension(filePath));
        var items = result.Items.Where(i => i.Information.Files != null && i.Information.Files.Count != 0).Select(i => new SubtitleEntry(i, laven.DistanceFrom(i.Information.Release), wantedLanguages))
            .OrderBy(i => i.LevenshteinDistance);

        return (items, result.Page, result.TotalPages);
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        using var client = GetClient();
        var info = await client.LoginAsync(username, password);

        long unixTimestamp = 0;
        try
        {
            var res = JwtParser.ParseClaimsFromJwt(info.Token).SingleOrDefault(c => c.Type == "exp");
            unixTimestamp = long.Parse(res.Value);
        }
        catch(Exception ex)
        {
            await Logger.LogExceptionAsync(ex);
            unixTimestamp = DateTime.UtcNow.ToUnixTimestamp();
        }

        return new User
        {
            Token = info.Token,
            AllowedDownloads = info.User.AllowedDownloads,
            IsVIP = info.User.Vip,
            Level = info.User.Level,
            UserId = info.User.UserId,
            Username = username,
            TokenExpirationUnixTimestamp = unixTimestamp
        };
    }

    public async Task<bool> LogoutAsync()
    {
        using var client = GetClient();

        try
        {
            return await client.LogoutAsync();
        }
        catch (RequestFailedException ex)
        {
            await Logger.LogExceptionAsync(ex);
            return false;
        }
    }

    public async Task<(IEnumerable<SubtitleEntry> Items, int CurrentPage, int TotalPages)> SearchSubtitlesAsync(
        string token,
        int? episodeNumber = null,
        int? seasonNumber = null,
        int? year = null,
        FileTypeFilter? filter = null,
        int? imdbId = null,
        int? parentImdbId = null,
        int currentPage = 1)
    {
        var parameters = _settings.Value.DefaultSearchParameters with
        {
            Languages = _settings.Value.WantedLanguages,
            OnlyMovieHashMatch = false,
            Query = token,
            EpisodeNumber = episodeNumber,
            SeasonNumber = seasonNumber,
            Year = year,
            Type = filter,
            ImdbId = imdbId,
            ParentImdbId = parentImdbId,
            Page = currentPage
        };

        using var newClient = GetClient(_settings.Value.ForceDefaultApiUrl);

        var result = await newClient.SearchAsync(parameters);

        var wantedLanguages = StaticResources.AllLanguages.Where(l => _settings.Value.WantedLanguages.Contains(l.Code));
        return (result.Items.Where(i => i.Information?.Files != null && i.Information.Files.Count != 0).Select(c => new SubtitleEntry(c, 0, wantedLanguages)), result.Page, result.TotalPages);
    }

    private static async Task<byte[]> GetRawFileAsync(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsByteArrayAsync()
            : null;
    }

    private OpenSubtitlesClient GetClient(bool forceDefaultUrl = false)
    {
        return new OpenSubtitlesClient(
            App.APIKey,
            _settings.Value.LoggedInUser?.Token,
            _settings.Value.LoggedInUser?.IsVIP == true && !forceDefaultUrl ? BaseUrlType.VIP : BaseUrlType.Default,
            App.UserAgent);
    }

    private string GetDestinationPath(string CurrentPath, string languageCode, string format)
    {
        format = format.StartsWith('.') ? format[1..] : format;

        var directoryPath = _settings.Value.DownloadToSubsFolder
            ? Path.Combine(Path.GetDirectoryName(CurrentPath), "Subs")
            : Path.GetDirectoryName(CurrentPath);

        Directory.CreateDirectory(directoryPath);

        if (_settings.Value.AllowMultipleDownloads)
        {
            var fileNameWithoutPathOrExtension = Path.GetFileNameWithoutExtension(CurrentPath);
            var path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.{languageCode}.{format}");

            if (!_settings.Value.OverwriteSameLanguageSub && File.Exists(path))
            {
                var counter = 1;
                while (File.Exists(path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.({counter}).{languageCode}.{format}")))
                {
                    counter++;
                }
            }

            return path;
        }

        return Path.ChangeExtension(CurrentPath, format);
    }
}
