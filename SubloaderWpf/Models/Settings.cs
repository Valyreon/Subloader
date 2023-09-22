using System.Collections.Generic;
using OpenSubtitlesSharp;

namespace SubloaderWpf.Utilities;

public class Settings
{
    public bool AllowMultipleDownloads { get; set; } = false;
    public bool DownloadToSubsFolder { get; set; } = false;
    public bool IsByHashChecked { get; set; } = true;
    public bool IsByNameChecked { get; set; } = false;
    public bool KeepWindowOnTop { get; set; } = true;
    public bool OverwriteSameLanguageSub { get; set; } = false;
    public string LoginToken { get; set; }
    public string BaseUrl { get; set; }

    public IReadOnlyList<string> WantedLanguages { get; set; } = new List<string>();
    public IReadOnlyList<string> Formats { get; set; } = new List<string>();
    public string PreferredFormat { get; set; } = "srt";
    public IReadOnlyList<SubtitleLanguage> AllLanguages { get; set; } = new List<SubtitleLanguage>();
}
