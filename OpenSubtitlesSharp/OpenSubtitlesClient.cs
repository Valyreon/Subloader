using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using OpenSubtitlesSharp.Dtos;
using OpenSubtitlesSharp.Extensions;
using RestSharp;

[assembly: InternalsVisibleTo("OpenSubtitlesSharp.Tests")]

namespace OpenSubtitlesSharp;

public class OpenSubtitlesClient : IOpenSubtitlesClient, IDisposable
{
    #region REGEXs
    private static readonly Regex _cleanRegex = new(@"[^\d\w\ ]");
    private static readonly Regex _multipleSpacesCleanRegex = new(@"\s+");
    private static readonly Regex _seasonEpisodeRegex = new(@"[sS](\d{1,2})[\s\.]?[eE](\d{1,2})");
    private static readonly Regex _yearRegex = new(@"(\.|\()\s*([12][90]\d{2})\s*(\.|\))");
    #endregion

    private readonly string _baseUrl = "https://api.opensubtitles.com/api/v1";
    private readonly IRestClient _client;

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
        _client = new RestClient(_baseUrl);
        _client.AddDefaultHeader("Api-Key", apiKey);
        _client.AddDefaultHeader("Authorization", "Bearer " + Token);
    }

    internal OpenSubtitlesClient(IRestClient client, string apiKey, string jwtToken = null) : this(apiKey, jwtToken)
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
        var request = new RestRequest("download", Method.Post);
        request.AddJsonBody(dlParameters);
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<DownloadInfo>();
    }

    public async Task<DownloadInfo> DownloadAsync(int fileId)
    {
        var request = new RestRequest("download", Method.Post);
        var dlParameters = new DownloadParameters { FileId = fileId };
        request.AddJsonBody(dlParameters);
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<DownloadInfo>();
    }

    public async Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync()
    {
        var request = new RestRequest("infos/languages");
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<LanguagesResponse>().Data;
    }

    public async Task<IReadOnlyList<string>> GetSubtitleFormatsAsync()
    {
        var request = new RestRequest("infos/formats");
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<SubtitleFormatsResponse>().Data.OutputFormats;
    }

    public async Task<UserInfo> GetUserInformationAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new RequestFailureException(System.Net.HttpStatusCode.BadRequest, "You have to provide JWT token for this endpoint.");
        }

        var request = new RestRequest("infos/user");
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<LoginInfo>().User;
    }

    public async Task<GuessResult> GuessPropertiesAsync(string fileName)
    {
        var request = new RestRequest("utilities/guessit");
        request.AddParameter("filename", fileName);
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<GuessResult>();
    }

    public async Task<LoginInfo> LoginAsync(string username, string password)
    {
        var request = new RestRequest("login", Method.Post);
        request.AddBody(new
        {
            username,
            password
        });
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<LoginInfo>();
    }

    public async Task<bool> LogoutAsync()
    {
        if (string.IsNullOrWhiteSpace(Token))
        {
            throw new RequestFailureException(System.Net.HttpStatusCode.BadRequest, "You have to provide JWT token for this endpoint.");
        }

        var request = new RestRequest("logout", Method.Delete);
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<MessageResponse>().Status == System.Net.HttpStatusCode.OK;
    }

    public async Task<SearchResult> SearchAsync(SearchParameters searchParameters)
    {
        var request = new RestRequest("subtitles");

        foreach (var kvp in searchParameters.ObjectToDictionary())
        {
            request.AddParameter(kvp.Key, kvp.Value);
        }

        // Execute the request and get the response
        var response = await _client.ExecuteAsync(request);
        return response.ParseResult<SearchResult>();
    }

    public Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null)
    {
        if (string.IsNullOrWhiteSpace(filePath) || !System.IO.File.Exists(filePath))
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
}
