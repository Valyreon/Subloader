using System;
using System.Collections.Generic;
using System.Linq;
using OpenSubtitlesSharp;
using SubloaderAvalonia.Extensions;

namespace SubloaderAvalonia.Models;

public class ApplicationSettings
{
    public static readonly IReadOnlyList<string> ValidFormats = new List<string> { "srt", "sub", "mpl", "webvtt", "dfxp", "txt" };

    private User loggedInUser;
    private SearchParameters defaultSearchParameters;
    private string preferredFormat = "srt";
    private IReadOnlyList<string> wantedLanguages;

    public bool AllowMultipleDownloads { get; set; }
    public SearchParameters DefaultSearchParameters
    {
        get => defaultSearchParameters ??= new();
        set => defaultSearchParameters = value;
    }
    public bool DownloadToSubsFolder { get; set; }
    public bool KeepWindowOnTop { get; set; } = true;

    public User LoggedInUser
    {
        get => loggedInUser == null || loggedInUser.TokenExpirationUnixTimestamp <= DateTime.UtcNow.ToUnixTimestamp() ? null : loggedInUser;
        set => loggedInUser = value;
    }

    public bool OverwriteSameLanguageSub { get; set; }

    public string PreferredFormat
    {
        get => ValidFormats.Contains(preferredFormat) ? preferredFormat : "srt";
        set => preferredFormat = value;
    }

    public IReadOnlyList<string> WantedLanguages
    {
        get => wantedLanguages == null || !wantedLanguages.Any() ? wantedLanguages = new List<string>() { "en" } : wantedLanguages;
        set => wantedLanguages = value;
    }
}
