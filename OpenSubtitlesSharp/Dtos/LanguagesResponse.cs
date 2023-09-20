using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

internal class LanguagesResponse
{
    [JsonPropertyName("data")]
    public List<SubtitleLanguage> Data { get; set; }
}
