using System.Collections.Generic;
using System.Linq;
using OpenSubtitlesSharp;
using SubloaderWpf.Mvvm;

namespace SubloaderWpf.Models;

public class SubtitleEntry : ObservableEntity
{
    public SubtitleEntry(Subtitle item, IEnumerable<SubtitleLanguage> allLanguages)
    {
        Model = item;
        Language = allLanguages.SingleOrDefault(l => string.Equals(l.Code, Model.Information.Language, System.StringComparison.InvariantCultureIgnoreCase))?.Name;
    }

    public string Language { get; }
    public bool IsHashMatch => Model.Information.IsHashMatch == true;
    public Subtitle Model { get; }
    public string Name => Model.Information.Release;
}
