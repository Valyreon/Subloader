using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OpenSubtitlesSharp;

namespace SubloaderWpf.Models;

public class ApplicationSettings
{
    public bool AllowMultipleDownloads { get; set; }
    public string BaseUrl { get; set; }
    public SearchParameters DefaultSearchParameters { get; set; } = new();
    public bool DownloadToSubsFolder { get; set; }
    public bool IsByHashChecked { get; set; } = true;
    public bool IsByNameChecked { get; set; }
    public bool KeepWindowOnTop { get; set; } = true;
    public UserInfo LatestUserInfo { get; set; }
    public string LoginToken { get; set; }
    public bool OverwriteSameLanguageSub { get; set; }
    public string PreferredFormat { get; set; } = "srt";

    [JsonIgnore]
    public IEnumerable<string> WantedLanguageCodes => WantedLanguages.Select(l => l.Code);

    public IReadOnlyList<SubtitleLanguage> WantedLanguages { get; set; } = new List<SubtitleLanguage>()
    {
        new SubtitleLanguage
        {
            Name = "English",
            Code = "en"
        }
    };
}
