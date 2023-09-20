using System.Text.Json.Serialization;

namespace OpenSubtitlesSharp;

public class FeatureDetails
{
    [JsonPropertyName("episode_number")]
    public int? EpisodeNumber { get; set; }

    [JsonPropertyName("feature_id")]
    public int? FeatureId { get; set; }

    [JsonPropertyName("feature_type")]
    public string FeatureType { get; set; }

    [JsonPropertyName("imdb_id")]
    public int? ImdbId { get; set; }

    [JsonPropertyName("movie_name")]
    public string MovieName { get; set; }

    [JsonPropertyName("parent_feature_id")]
    public int? ParentFeatureId { get; set; }

    [JsonPropertyName("parent_imdb_id")]
    public int? ParentImdbId { get; set; }

    [JsonPropertyName("parent_title")]
    public string ParentTitle { get; set; }

    [JsonPropertyName("parent_tmdb_id")]
    public int? ParentTmdbId { get; set; }

    [JsonPropertyName("season_number")]
    public int? SeasonNumber { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("tmdb_id")]
    public int? TmdbId { get; set; }

    [JsonPropertyName("year")]
    public int? Year { get; set; }
}

public class SubtitleFileInfo
{
    [JsonPropertyName("cd_number")]
    public int? CdNumber { get; set; }

    [JsonPropertyName("file_id")]
    public int? FileId { get; set; }

    [JsonPropertyName("file_name")]
    public string FileName { get; set; }
}

public class RelatedLink
{
    [JsonPropertyName("img_url")]
    public string ImgUrl { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }
}

public class SearchResult
{
    [JsonPropertyName("data")]
    public List<Subtitle> Items { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }
}

public class Subtitle
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("attributes")]
    public SubtitleInformation Information { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public class SubtitleInformation
{
    [JsonPropertyName("ai_translated")]
    public bool? AiTranslated { get; set; }

    [JsonPropertyName("comments")]
    public string Comments { get; set; }

    [JsonPropertyName("download_count")]
    public int? DownloadCount { get; set; }

    [JsonPropertyName("feature_details")]
    public FeatureDetails FeatureDetails { get; set; }

    [JsonPropertyName("files")]
    public List<SubtitleFileInfo> Files { get; set; }

    [JsonPropertyName("foreign_parts_only")]
    public bool? ForeignPartsOnly { get; set; }

    [JsonPropertyName("fps")]
    public double Fps { get; set; }

    [JsonPropertyName("from_trusted")]
    public bool? FromTrusted { get; set; }

    [JsonPropertyName("hd")]
    public bool? Hd { get; set; }

    [JsonPropertyName("hearing_impaired")]
    public bool? HearingImpaired { get; set; }

    [JsonPropertyName("language")]
    public string Language { get; set; }

    [JsonPropertyName("legacy_subtitle_id")]
    public int? LegacySubtitleId { get; set; }

    [JsonPropertyName("machine_translated")]
    public bool? MachineTranslated { get; set; }

    [JsonPropertyName("new_download_count")]
    public int? NewDownloadCount { get; set; }

    [JsonPropertyName("ratings")]
    public double Ratings { get; set; }

    [JsonPropertyName("related_links")]
    public List<RelatedLink> RelatedLinks { get; set; }

    [JsonPropertyName("release")]
    public string Release { get; set; }

    [JsonPropertyName("subtitle_id")]
    public string SubtitleId { get; set; }

    [JsonPropertyName("upload_date")]
    public DateTime UploadDate { get; set; }

    [JsonPropertyName("uploader")]
    public Uploader Uploader { get; set; }

    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("votes")]
    public int? Votes { get; set; }
}

public class Uploader
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("rank")]
    public string Rank { get; set; }

    [JsonPropertyName("uploader_id")]
    public int? UploaderId { get; set; }
}
