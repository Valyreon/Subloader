using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

internal class SubtitleFormatsData
{
    [JsonPropertyName("output_formats")]
    public List<string> OutputFormats { get; set; }
}

internal class SubtitleFormatsResponse
{
    [JsonPropertyName("data")]
    public SubtitleFormatsData Data { get; set; }
}
