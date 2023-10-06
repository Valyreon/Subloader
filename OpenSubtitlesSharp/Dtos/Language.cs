using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class SubtitleLanguage
{
    [JsonPropertyName("language_code")]
    public string Code { get; set; }

    [JsonPropertyName("language_name")]
    public string Name { get; set; }
}
