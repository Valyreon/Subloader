using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class DownloadParameters
{
    [JsonPropertyName("file_id")]
    public int FileId { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    [JsonPropertyName("force_download")]
    public bool? ForceDownload { get; set; }

    [JsonPropertyName("in_fps")]
    public int? InFPS { get; set; }

    [JsonPropertyName("out_fps")]
    public int? OutFPS { get; set; }

    [JsonPropertyName("sub_format")]
    public string SubFormat { get; set; }

    [JsonPropertyName("timeshift")]
    public decimal? Timeshift { get; set; }
}
