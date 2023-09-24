using System.Collections.Generic;
using System.Linq;
using OpenSubtitlesSharp;
using SubloaderWpf.Mvvm;

namespace SubloaderWpf.Models;

public class SubtitleEntry : ObservableEntity
{
    public SubtitleEntry(Subtitle item, IEnumerable<SubtitleLanguage> allLanguages, bool byHash)
    {
        Model = item;
        Language = allLanguages.SingleOrDefault(l => l.Code == Model.Information.Language)?.Name;
        ByHash = byHash;
    }

    public string Language { get; }
    public bool ByHash { get; }
    public Subtitle Model { get; }
    public string Name => Model.Information.Release;
}
