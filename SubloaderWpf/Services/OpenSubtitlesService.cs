using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Fastenshtein;
using OpenSubtitlesSharp;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.Services;

public class OpenSubtitlesService : IOpenSubtitlesService
{
    private readonly ApplicationSettings _settings;

    public OpenSubtitlesService(ApplicationSettings settings)
    {
        _settings = settings;
    }

    public async Task<DownloadInfo> DownloadSubtitleAsync(SubtitleEntry subtitle, string videoPath, string savePath = null)
    {
        using var osClient = GetClient();
        var downloadInfo = await osClient.GetDownloadInfoAsync(subtitle.Model.Information.Files.First().FileId.Value);
        var extension = Path.GetExtension(downloadInfo.FileName);

        var destination = string.IsNullOrWhiteSpace(savePath)
            ? GetDestinationPath(videoPath, subtitle.Model.Information.Language, extension)
            : savePath;

        File.WriteAllBytes(destination, await GetRawFileAsync(downloadInfo.Link));

        if (_settings.LoggedInUser != null)
        {
            _settings.LoggedInUser.ResetTime = downloadInfo.ResetTimeUtc;
            _settings.LoggedInUser.RemainingDownloads = downloadInfo.Remaining;

            _ = SettingsParser.SaveAsync(_settings);
        }

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

    public async Task<IEnumerable<SubtitleEntry>> GetSubtitlesForFileAsync(string filePath, bool searchByName, bool searchByHash)
    {
        var parameters = _settings.DefaultSearchParameters with
        {
            Languages = _settings.WantedLanguages.Select(l => l.Code),
            OnlyMovieHashMatch = searchByHash && !searchByName
        };

        using var newClient = GetClient();

        var result = await newClient.SearchAsync(filePath, parameters);

        // order by levenshtein distance
        var laven = new Levenshtein(Path.GetFileNameWithoutExtension(filePath));
        return result.Items.Select(i => new SubtitleEntry(i, _settings.WantedLanguages))
            .Select(ResultItem => (ResultItem, laven.DistanceFrom(ResultItem.Name)))
            .OrderBy(i => i.Item2)
            .Select(i => i.ResultItem);
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        using var client = GetClient();
        var info = await client.LoginAsync(username, password);

        return new User
        {
            Token = info.Token,
            BaseUrl = info.BaseUrl,
            AllowedDownloads = info.User.AllowedDownloads,
            RemainingDownloads = info.User.RemainingDownloads,
            IsVIP = info.User.Vip,
            Level = info.User.Level,
            UserId = info.User.UserId,
            Username = username
        };
    }

    public async Task<bool> LogoutAsync()
    {
        using var client = GetClient();

        try
        {
            return await client.LogoutAsync();
        }
        catch (RequestFailedException)
        {
            return false;
        }
    }

    public async Task<IEnumerable<SubtitleEntry>> SearchSubtitlesAsync(string token, int? episodeNumber = null, int? seasonNumber = null, int? year = null, FileTypeFilter? filter = null, int? imdbId = null, int? parentImdbId = null)
    {
        var parameters = _settings.DefaultSearchParameters with
        {
            Languages = _settings.WantedLanguageCodes,
            OnlyMovieHashMatch = false,
            Query = token,
            EpisodeNumber = episodeNumber,
            SeasonNumber = seasonNumber,
            Year = year,
            Type = filter,
            ImdbId = imdbId,
            ParentImdbId = parentImdbId,
        };

        using var newClient = GetClient();

        var result = await newClient.SearchAsync(parameters);

        return result.Items.Select(c => new SubtitleEntry(c, _settings.WantedLanguages));
    }

    private static async Task<byte[]> GetRawFileAsync(string url)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);

        return response.IsSuccessStatusCode
            ? await response.Content.ReadAsByteArrayAsync()
            : null;
    }

    private OpenSubtitlesClient GetClient()
    {
        return new OpenSubtitlesClient(App.APIKey, _settings.LoggedInUser?.Token, _settings.LoggedInUser?.BaseUrl);
    }

    private string GetDestinationPath(string CurrentPath, string languageCode, string format)
    {
        format = format.StartsWith(".") ? format[1..] : format;

        var directoryPath = _settings.DownloadToSubsFolder
            ? Path.Combine(Path.GetDirectoryName(CurrentPath), "Subs")
            : Path.GetDirectoryName(CurrentPath);

        Directory.CreateDirectory(directoryPath);

        if (_settings.AllowMultipleDownloads)
        {
            var fileNameWithoutPathOrExtension = Path.GetFileNameWithoutExtension(CurrentPath);
            var path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.{languageCode}.{format}");

            if (!_settings.OverwriteSameLanguageSub && File.Exists(path))
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
