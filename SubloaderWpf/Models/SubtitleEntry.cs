using System.Collections.Generic;
using System.Linq;
using OpenSubtitlesSharp;
using SubloaderWpf.Mvvm;

namespace SubloaderWpf.Models;

public class SubtitleEntry : ObservableEntity
{
    public SubtitleEntry(Subtitle item, IEnumerable<SubtitleLanguage> allLanguages)
    {
        IsHashMatch = item.Information.IsHashMatch == true;
        FileId = item.Information.Files[0].FileId.Value;
        Name = item.Information.Release;
        var lang = allLanguages.SingleOrDefault(l => string.Equals(l.Code, item.Information.Language, System.StringComparison.InvariantCultureIgnoreCase));
        Language = lang?.Name;
        LanguageCode = lang?.Code;
    }

    public string Language { get; }
    public string LanguageCode { get; }
    public bool IsHashMatch { get; }
    public int FileId { get; }
    public string Name { get; }
}
