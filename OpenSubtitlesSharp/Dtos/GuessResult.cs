using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class GuessResult
{
    [JsonPropertyName("audio_channels")]
    public string AudioChannels { get; set; }

    [JsonPropertyName("audio_codec")]
    public string AudioCodec { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("other")]
    public string Other { get; set; }

    [JsonPropertyName("release_group")]
    public string ReleaseGroup { get; set; }

    [JsonPropertyName("screen_size")]
    public string ScreenSize { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("streaming_service")]
    public string StreamingService { get; set; }

    [JsonPropertyName("subtitle_language")]
    public string SubtitleLanguage { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("video_codec")]
    public string VideoCodec { get; set; }

    [JsonPropertyName("year")]
    public int Year { get; set; }
}
