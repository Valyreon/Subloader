namespace OpenSubtitlesSharp;

public interface IOpenSubtitlesClient
{
    string Token { get; }

    Task<DownloadInfo> GetDownloadInfoAsync(DownloadParameters dlParameters);

    Task<DownloadInfo> GetDownloadInfoAsync(int fileId);

    Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync();

    Task<IReadOnlyList<string>> GetSubtitleFormatsAsync();

    Task<UserInfo> GetUserInfoAsync();

    Task<LoginInfo> LoginAsync(string username, string password);

    Task<bool> LogoutAsync();

    Task<SearchResult> SearchAsync(SearchParameters searchParameters);

    Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null);

    SearchParameters ParseFileNameForSearchParameters(string filePath, SearchParameters initialParameters = null);
}
