using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using OpenSubtitlesSharp.Extensions;
using OpenSubtitlesSharp.Interfaces;
using OpenSubtitlesSharp.Services;

[assembly: InternalsVisibleTo("OpenSubtitlesSharp.Tests")]

namespace OpenSubtitlesSharp;

public enum BaseUrlType
{
    Default,
    VIP
}

public class OpenSubtitlesClient : IOpenSubtitlesClient, IDisposable
{
    #region REGEXs

    private static readonly Regex _cleanRegex = new(@"[^\d\w\ ]");
    private static readonly Regex _multipleSpacesCleanRegex = new(@"\s+");
    private static readonly Regex _seasonEpisodeRegex = new(@"[sS](\d{1,2})[\s\.]?[eE](\d{1,2})");
    private static readonly Regex _yearRegex = new(@"\(\s*([12][90]\d{2})\s*\)");

    #endregion REGEXs

    private static readonly string _defaultBaseUrl = "https://api.opensubtitles.com/api/v1/";
    private static readonly string _vipBaseUrl = "https://vip-api.opensubtitles.com/api/v1/";
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly IFileSystemService _fileSystemService;
    private readonly IMovieHasherService _movieHasherService;
    private readonly string _baseUrl = "https://api.opensubtitles.com/api/v1/";
    private readonly HttpClient _client;

    public OpenSubtitlesClient(string apiKey, string jwtToken = null, BaseUrlType baseUrlType = BaseUrlType.Default)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), "API Key has to be specified.");
        }

        _baseUrl = baseUrlType switch
        {
            BaseUrlType.VIP => _vipBaseUrl,
            _ => _defaultBaseUrl
        };

        Token = jwtToken;
        _fileSystemService = new FileSystemService();
        _movieHasherService = new MovieHasherService(_fileSystemService);
        _client ??= new HttpClient()
        {
            BaseAddress = new Uri(_baseUrl)
        };
        _client.DefaultRequestHeaders.Add("Api-Key", apiKey);

        if (!string.IsNullOrWhiteSpace(Token))
        {
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        }

        _client.DefaultRequestHeaders.Add("User-Agent", "Subloader v1.5");
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        //_client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
    }

    public OpenSubtitlesClient(string apiKey, string jwtToken = null, bool isVip = false)
        : this(apiKey, jwtToken, isVip ? BaseUrlType.VIP : BaseUrlType.Default)
    {

    }

    internal OpenSubtitlesClient(HttpClient client, IFileSystemService fileSystemService, IMovieHasherService movieHasherService, string apiKey, string jwtToken = null) : this(apiKey, jwtToken, false)
    {
        _client = client;
        _client.BaseAddress = new Uri(_baseUrl);
        _fileSystemService = fileSystemService;
        _movieHasherService = movieHasherService;
    }

    public string Token { get; private set; }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<DownloadInfo> GetDownloadInfoAsync(DownloadParameters dlParameters)
    {
        var body = JsonSerializer.Serialize(dlParameters, _jsonSerializerOptions);
        var content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("download", content);
        return await response.ParseResultAsync<DownloadInfo>();
    }

    public Task<DownloadInfo> GetDownloadInfoAsync(int fileId)
    {
        var dlParameters = new DownloadParameters { FileId = fileId };
        return GetDownloadInfoAsync(dlParameters);
    }

    public async Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync()
    {
        var response = await _client.GetAsync("infos/languages");
        var result = await response.ParseResultAsync<LanguagesResponse>();
        return result.Data;
    }

    public async Task<IReadOnlyList<string>> GetSubtitleFormatsAsync()
    {
        var response = await _client.GetAsync("infos/formats");
        var result = await response.ParseResultAsync<SubtitleFormatsResponse>();
        return result.Data.OutputFormats;
    }

    public async Task<UserInfo> GetUserInfoAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new InvalidOperationException("You have to be logged in to call this endpoint.");
        }

        var response = await _client.GetAsync("infos/user");
        var result = await response.ParseResultAsync<UserInfoResponse>();
        return result.Data;
    }

    public async Task<LoginInfo> LoginAsync(string username, string password)
    {
        var bodyObject = new
        {
            username,
            password
        };

        var content = new StringContent(JsonSerializer.Serialize(bodyObject), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("login", content);
        var info = await response.ParseResultAsync<LoginInfo>();

        Token = info.Token;
        _client.DefaultRequestHeaders.Remove("Authorization");
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

        return info;
    }

    public async Task<bool> LogoutAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new InvalidOperationException("You have to be logged in to call this endpoint.");
        }

        var response = await _client.DeleteAsync("logout");
        var result = await response.ParseResultAsync<MessageResponse>();
        return result.Status == HttpStatusCode.OK;
    }

    public async Task<SearchResult> SearchAsync(SearchParameters searchParameters)
    {
        var queryUrl = BuildGetQueryURL(searchParameters.ObjectToDictionary());
        var response = await _client.GetAsync("subtitles" + queryUrl);
        return await response.ParseResultAsync<SearchResult>();
    }

    public Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null)
    {
        parameters = ParseFileNameForSearchParameters(filePath, parameters);

        return SearchAsync(parameters);
    }

    private string BuildGetQueryURL(IReadOnlyDictionary<string, string> paramsDict)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        foreach (var kvp in paramsDict.OrderBy(c => c.Key))
        {
            query.Add(kvp.Key, kvp.Value);
        }

        return "?" + query;
    }

    internal SearchParameters ParseFileNameForSearchParameters(string filePath, SearchParameters initialParameters = null)
    {
        var parameters = initialParameters;

        if (string.IsNullOrWhiteSpace(filePath) || !_fileSystemService.Exists(filePath))
        {
            throw new ArgumentException("The file path must be valid and exist.", nameof(filePath));
        }

        parameters ??= new SearchParameters();
        parameters.MovieHash = _movieHasherService.ComputeMovieHash(filePath);

        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(filePath);

        // try to extract season and episode info from file name
        var seasonEpisodeInfo = _seasonEpisodeRegex.Match(fileNameWithoutExt);
        if (seasonEpisodeInfo.Success)
        {
            var season = int.Parse(seasonEpisodeInfo.Groups[1].Value);
            var episode = int.Parse(seasonEpisodeInfo.Groups[2].Value);

            parameters.SeasonNumber = season == 0 ? null : season;
            parameters.EpisodeNumber = episode == 0 ? null : episode;
            parameters.Type = FileTypeFilter.Episode;

            fileNameWithoutExt = _seasonEpisodeRegex.Replace(fileNameWithoutExt, string.Empty);
        }

        // tv shows usually dont have year in the episode name so we will ignore year
        // when episode info is found
        if (!seasonEpisodeInfo.Success)
        {
            // try to extract year info from file name
            var yearInfo = _yearRegex.Match(fileNameWithoutExt);
            if (yearInfo.Success)
            {
                var year = int.Parse(yearInfo.Groups[1].Value);

                if (year > 1900 && year <= DateTime.Now.Year)
                {
                    parameters.Year = year;

                    fileNameWithoutExt = _yearRegex.Replace(fileNameWithoutExt, string.Empty);
                }
            }
        } 

        parameters.Query = _multipleSpacesCleanRegex.Replace(_cleanRegex.Replace(fileNameWithoutExt, " "), " ").Trim();

        return parameters;
    }
}
