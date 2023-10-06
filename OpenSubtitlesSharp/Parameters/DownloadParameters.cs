using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class DownloadParameters
{
    /// <summary>
    /// File ID of the file to get download link for, gotten from search results.
    /// </summary>
    [JsonPropertyName("file_id")]
    public int FileId { get; set; }

    /// <summary>
    /// Desired file name.
    /// </summary>
    [JsonPropertyName("file_name")]
    public string FileName { get; set; }

    /// <summary>
    /// Set subtitle file headers to "application/force-download"
    /// </summary>
    [JsonPropertyName("force_download")]
    public bool? ForceDownload { get; set; }

    /// <summary>
    /// Used for conversions, both InFPS and OutFPS must then be indicated.
    /// </summary>
    [JsonPropertyName("in_fps")]
    public int? InFPS { get; set; }

    /// <summary>
    /// Used for conversions, both InFPS and OutFPS must then be indicated.
    /// </summary>
    [JsonPropertyName("out_fps")]
    public int? OutFPS { get; set; }

    /// <summary>
    /// Desired subtitle format. You can get available formats from the formats info endpoint.
    /// </summary>
    [JsonPropertyName("sub_format")]
    public string SubFormat { get; set; }

    /// <summary>
    /// Specify if you want to modify the subtitle timestamps, delay or speed up the subtitles.
    /// </summary>
    [JsonPropertyName("timeshift")]
    public decimal? Timeshift { get; set; }
}
