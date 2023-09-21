using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using OpenSubtitlesSharp.Dtos;
using OpenSubtitlesSharp.Extensions;

[assembly: InternalsVisibleTo("OpenSubtitlesSharp.Tests")]

namespace OpenSubtitlesSharp;

public class OpenSubtitlesClient : IOpenSubtitlesClient, IDisposable
{
    #region REGEXs

    private static readonly Regex _cleanRegex = new(@"[^\d\w\ ]");
    private static readonly Regex _multipleSpacesCleanRegex = new(@"\s+");
    private static readonly Regex _seasonEpisodeRegex = new(@"[sS](\d{1,2})[\s\.]?[eE](\d{1,2})");
    private static readonly Regex _yearRegex = new(@"(\.|\()\s*([12][90]\d{2})\s*(\.|\))");

    #endregion REGEXs

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly string _baseUrl = "https://api.opensubtitles.com/api/v1";
    private readonly HttpClient _client;

    public OpenSubtitlesClient(string apiKey, string jwtToken = null, string baseUrl = null)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentNullException(nameof(apiKey), "API Key has to be specified.");
        }

        if (!string.IsNullOrWhiteSpace(baseUrl) && baseUrl != "api.opensubtitles.com" && baseUrl != "vip-api.opensubtitles.com")
        {
            throw new ArgumentException(nameof(baseUrl), "Invalid base url value. Use the one provided by login endpoint.");
        }

        if (!string.IsNullOrWhiteSpace(baseUrl))
        {
            _baseUrl = $"https://{baseUrl}/api/v1";
        }

        Token = jwtToken;
        _client = new HttpClient();
        _client.DefaultRequestHeaders.Add("Api-Key", apiKey);
        _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);
        _client.DefaultRequestHeaders.Add("User-Agent", "RestSharp/110.2.0.0");
    }

    internal OpenSubtitlesClient(HttpClient client, string apiKey, string jwtToken = null) : this(apiKey, jwtToken)
    {
        _client = client;
    }

    public string Token { get; private set; }

    public void Dispose()
    {
        _client.Dispose();
    }

    public async Task<DownloadInfo> DownloadAsync(DownloadParameters dlParameters)
    {
        var body = JsonSerializer.Serialize(dlParameters, _jsonSerializerOptions);
        var content = new StringContent(body);
        content.Headers.ContentType = null;
        var response = await _client.PostAsync(GetResourceURL("download"), content);
        return await response.ParseResultAsync<DownloadInfo>();
    }

    public Task<DownloadInfo> DownloadAsync(int fileId)
    {
        var dlParameters = new DownloadParameters { FileId = fileId };
        return DownloadAsync(dlParameters);
    }

    public async Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync()
    {
        var response = await _client.GetAsync(GetResourceURL("infos/languages"));
        var result = await response.ParseResultAsync<LanguagesResponse>();
        return result.Data;
    }

    public async Task<IReadOnlyList<string>> GetSubtitleFormatsAsync()
    {
        var response = await _client.GetAsync(GetResourceURL("infos/formats"));
        var result = await response.ParseResultAsync<SubtitleFormatsResponse>();
        return result.Data.OutputFormats;
    }

    public async Task<UserInfo> GetUserInformationAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new RequestFailureException(HttpStatusCode.BadRequest, "You have to provide JWT token for this endpoint.");
        }

        var response = await _client.GetAsync(GetResourceURL("infos/user"));
        var result = await response.ParseResultAsync<LoginInfo>();
        return result.User;
    }

    public async Task<GuessResult> GuessPropertiesAsync(string fileName)
    {
        var response = await _client.GetAsync(BuildGetQueryURL(GetResourceURL("utilities/guessit"), "filename", fileName));
        return await response.ParseResultAsync<GuessResult>();
    }

    public async Task<LoginInfo> LoginAsync(string username, string password)
    {
        var bodyObject = new
        {
            username,
            password
        };

        var content = new StringContent(JsonSerializer.Serialize(bodyObject), Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(GetResourceURL("login"), content);
        return await response.ParseResultAsync<LoginInfo>();
    }

    public async Task<bool> LogoutAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new RequestFailureException(HttpStatusCode.BadRequest, "You have to provide JWT token for this endpoint.");
        }

        var response = await _client.DeleteAsync(GetResourceURL("logout"));
        var result = await response.ParseResultAsync<MessageResponse>();
        return result.Status == HttpStatusCode.OK;
    }

    public async Task<SearchResult> SearchAsync(SearchParameters searchParameters)
    {
        var queryUrl = BuildGetQueryURL(GetResourceURL("subtitles"), searchParameters.ObjectToDictionary());
        var response = await _client.GetAsync(queryUrl);
        return await response.ParseResultAsync<SearchResult>();
    }

    public Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
        {
            throw new ArgumentException("The file path must be valid and exist.", nameof(filePath));
        }

        parameters ??= new SearchParameters();
        parameters.MovieHash = MovieHasher.ComputeMovieHash(filePath);

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
        }

        // try to extract year info from file name
        var yearInfo = _yearRegex.Match(fileNameWithoutExt);
        if (yearInfo.Success)
        {
            var year = int.Parse(yearInfo.Groups[2].Value);

            if (year > 1900 && year <= DateTime.Now.Year)
            {
                parameters.Year = year;
            }
        }

        parameters.Query = _multipleSpacesCleanRegex.Replace(_cleanRegex.Replace(fileNameWithoutExt, " "), " ");

        return SearchAsync(parameters);
    }

    internal string BuildGetQueryURL(string urlString, IReadOnlyDictionary<string, string> paramsDict)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        foreach (var kvp in paramsDict)
        {
            query.Add(kvp.Key, kvp.Value);
        }

        return urlString + "?" + query.ToString();
    }

    internal string BuildGetQueryURL(string urlString, string key, string value)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query[key] = value;
        return urlString + "?" + query.ToString();
    }

    internal string GetResourceURL(params string[] parts)
    {
        return _baseUrl + "/" + string.Join("/", parts);
    }
}
