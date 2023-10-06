namespace OpenSubtitlesSharp;

/// <summary>
/// Represents a client for accessing OpenSubtitles REST API.
/// </summary>
public interface IOpenSubtitlesClient
{
    string Token { get; }

    /// <summary>
    /// Request a download URL for a subtitle. The download count is calculated on this action.
    /// </summary>
    /// <param name="dlParameters">Parameters for download.</param>
    /// <returns>Object containing download URL and other download info.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<DownloadInfo> GetDownloadInfoAsync(DownloadParameters dlParameters);

    /// <summary>
    /// Request a download URL for a subtitle. The download count is calculated on this action.
    /// </summary>
    /// <param name="fileId">File ID of the subtitle to download.</param>
    /// <returns>Object containing download URL and other download info.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<DownloadInfo> GetDownloadInfoAsync(int fileId);

    /// <summary>
    /// Get the languages table containing all languages and their codes.
    /// </summary>
    /// <returns>List of available languages on OS API.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<IReadOnlyList<SubtitleLanguage>> GetLanguagesAsync();

    /// <summary>
    /// Get the list of subtitle formats recognized by the API.
    /// </summary>
    /// <returns>List of supported subtitle formats.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<IReadOnlyList<string>> GetSubtitleFormatsAsync();

    /// <summary>
    /// Gets information about the currently logged in User.
    /// </summary>
    /// <returns>Logged in user information.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<UserInfo> GetUserInfoAsync();

    /// <summary>
    /// Used to login to OpenSubtitles.com.
    /// </summary>
    /// <param name="username">Username.</param>
    /// <param name="password">Password.</param>
    /// <returns>Token and info about the logged in user.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<LoginInfo> LoginAsync(string username, string password);

    /// <summary>
    /// Used for logout.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<bool> LogoutAsync();

    /// <summary>
    /// Search for subtitles based on the parameters.
    /// </summary>
    /// <param name="searchParameters">Parameters for subtitle search.</param>
    /// <returns>A page of search results.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<SearchResult> SearchAsync(SearchParameters searchParameters);

    /// <summary>
    /// Search for subtitles for a specific video file.
    /// </summary>
    /// <param name="filePath">File path of the video file.</param>
    /// <param name="parameters">Initial search parameters.</param>
    /// <returns>A page of search results.</returns>
    /// <exception cref="RequestFailedException">Thrown when the REST API responds with a non-200 status code.</exception>
    Task<SearchResult> SearchAsync(string filePath, SearchParameters parameters = null);
}
