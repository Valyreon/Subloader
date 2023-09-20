using OpenSubtitlesSharp.Dtos;

namespace OpenSubtitlesSharp;

public interface IOpenSubtitlesClient
{
    string Token { get; }

    Task<DownloadInfo> DownloadAsync(DownloadParameters dlParameters);

    Task<DownloadInfo> DownloadAsync(int fileId);

    Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync();

    Task<IReadOnlyList<string>> GetSubtitleFormatsAsync();

    Task<UserInfo> GetUserInformationAsync();

    Task<GuessResult> GuessPropertiesAsync(string fileName);

    Task<LoginInfo> LoginAsync(string username, string password);

    Task<bool> LogoutAsync();

    Task<SearchResult> SearchAsync(SearchParameters searchParameters);

    Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null);
}
