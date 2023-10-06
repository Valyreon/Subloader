using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class DownloadInfo
{
    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    [JsonPropertyName("link")]
    public string Link { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("remaining")]
    public int Remaining { get; set; }

    [JsonPropertyName("requests")]
    public int Requests { get; set; }

    [JsonPropertyName("reset_time")]
    public string ResetTime { get; set; }

    [JsonPropertyName("reset_time_utc")]
    public DateTime ResetTimeUtc { get; set; }
}
