using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OpenSubtitlesSharp;

namespace SubloaderWpf.Models;

public class ApplicationSettings
{
    public bool AllowMultipleDownloads { get; set; }
    public SearchParameters DefaultSearchParameters { get; set; } = new();
    public bool DownloadToSubsFolder { get; set; }
    public bool KeepWindowOnTop { get; set; } = true;
    public User LoggedInUser { get; set; }
    public bool OverwriteSameLanguageSub { get; set; }
    public string PreferredFormat { get; set; } = "srt";

    [JsonIgnore]
    public IEnumerable<string> WantedLanguageCodes => WantedLanguages.Select(l => l.Code);

    public IReadOnlyList<SubtitleLanguage> WantedLanguages { get; set; }

    public ApplicationSettings Initialize()
    {
        DefaultSearchParameters ??= new();
        PreferredFormat ??= "srt";
        WantedLanguages ??= new List<SubtitleLanguage>()
        {
            new SubtitleLanguage
            {
                Name = "English",
                Code = "en"
            }
        };

        if (LoggedInUser?.ResetTime.HasValue == true && LoggedInUser.ResetTime.Value <= DateTime.UtcNow)
        {
            LoggedInUser.ResetTime = null;
        }

        return this;
    }
}
