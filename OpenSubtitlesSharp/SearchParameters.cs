using OpenSubtitlesSharp.Attributes;
using OpenSubtitlesSharp.DictionaryConverters;

namespace OpenSubtitlesSharp;

public enum FileTypeFilter
{
    Movie,
    Episode,
    All
}

public enum Filter
{
    Include,
    Exclude,
    Only
}

public class SearchParameters
{
    /// <summary>
    /// Episode number for TV Shows.
    /// </summary>
    [DictionaryValue("episode_number")]
    public int? EpisodeNumber { get; set; }

    /// <summary>
    /// Filter that determines whether subtitles where only foreign parts are translated will be included. Default is Include.
    /// </summary>
    [DictionaryValue("foreign_parts_only")]
    public Filter? ForeignPartsOnly { get; set; }

    /// <summary>
    /// Filter that determines whether subtitles for hearing impaired will be returned. Default is Include.
    /// </summary>
    [DictionaryValue("hearing_impaired")]
    public Filter? HearingImpaired { get; set; }

    /// <summary>
    /// ID of the movie of episode.
    /// </summary>
    [DictionaryValue("id")]
    public int? Id { get; set; }

    /// <summary>
    /// IMDB Id of movie or episode.
    /// </summary>
    [DictionaryValue("imdb_id")]
    public int? ImdbId { get; set; }

    /// <summary>
    /// If true, search results will include AI translated subtitles.
    /// </summary>
    [DictionaryValue("ai_translated", typeof(ExcludeIncludeValueConverter))]
    public bool? IncludeAiTranslated { get; set; }

    /// <summary>
    /// If true, search results will include Machine translated subtitles.
    /// </summary>
    [DictionaryValue("machine_translated")]
    public bool? IncludeMachineTranslated { get; set; }

    /// <summary>
    /// If true, search results will include only subtitles from trusted sources.
    /// </summary>
    [DictionaryValue("trusted_sources", typeof(IncludeOnlyValueConverter))]
    public bool? IncludeOnlyFromTrustedSources { get; set; }

    /// <summary>
    /// List of language codes to filter.
    /// </summary>
    [DictionaryValue("languages", typeof(OrderedCsvValueConverter))]
    public IReadOnlyList<string> Languages { get; set; }

    /// <summary>
    /// Moviehash of the movie/episode file.
    /// </summary>
    [DictionaryValue("moviehash")]
    public string MovieHash { get; set; }

    /// <summary>
    /// If true, results will include only subtitles which matched the MovieHash.
    /// </summary>
    [DictionaryValue("moviehash_match", typeof(IncludeOnlyValueConverter))]
    public bool? OnlyMovieHashMatch { get; set; }

    /// <summary>
    /// Results page to return.
    /// </summary>
    [DictionaryValue("page")]
    public int? Page { get; set; }

    /// <summary>
    /// For TV Shows.
    /// </summary>
    [DictionaryValue("parent_feature_id")]
    public int? ParentFeatureId { get; set; }

    /// <summary>
    /// For TV Shows.
    /// </summary>
    [DictionaryValue("parent_imdb_id")]
    public int? ParentImdbId { get; set; }

    /// <summary>
    /// For TV Shows.
    /// </summary>
    [DictionaryValue("parent_tmdb_id")]
    public int? ParentTmdbId { get; set; }

    /// <summary>
    /// File name or text search.
    /// </summary>
    [DictionaryValue("query")]
    public string Query { get; set; }

    /// <summary>
    /// For TV Shows.
    /// </summary>
    [DictionaryValue("season_number")]
    public int? SeasonNumber { get; set; }

    /// <summary>
    /// TMDB Id of movie or episode.
    /// </summary>
    [DictionaryValue("tmdb_id")]
    public int? TmdbId { get; set; }

    /// <summary>
    /// Type of the file.
    /// </summary>
    [DictionaryValue("type")]
    public FileTypeFilter? Type { get; set; }

    /// <summary>
    /// If you want to filter by year.
    /// </summary>
    [DictionaryValue("year")]
    public int? Year { get; set; }
}
