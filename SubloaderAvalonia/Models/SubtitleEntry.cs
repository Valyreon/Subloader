using System.Collections.Generic;
using System.Linq;
using OpenSubtitlesSharp;

namespace SubloaderAvalonia.Models;

public class SubtitleEntry
{
    public SubtitleEntry(Subtitle item, int levenDistance, IEnumerable<SubtitleLanguage> allLanguages)
    {
        IsHashMatch = item.Information.IsHashMatch == true;
        FileId = item.Information.Files[0].FileId.Value;
        Release = item.Information.Release;
        var lang = allLanguages.SingleOrDefault(l => string.Equals(l.Code, item.Information.Language, System.StringComparison.InvariantCultureIgnoreCase));
        Language = lang?.Name;
        LanguageCode = lang?.Code;
        LevenshteinDistance = levenDistance;
    }

    public string Language { get; }
    public string LanguageCode { get; }
    public bool IsHashMatch { get; }
    public int FileId { get; }
    public string Release { get; }
    public int LevenshteinDistance { get; }
}
