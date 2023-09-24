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
        Language = allLanguages.SingleOrDefault(l => l.Code == Model.Information.Language)?.Name;
    }

    public string Language { get; }
    public Subtitle Model { get; }
    public string Name => Model.Information.Release;
}
