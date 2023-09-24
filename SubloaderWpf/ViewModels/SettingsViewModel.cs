using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OpenSubtitlesSharp;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Mvvm;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class SettingsViewModel : ObservableEntity
{
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
    private readonly IEnumerable<SubtitleLanguage> allLanguages;
    private readonly INavigator navigator;
    private bool allowMultipleDownloads;
    private bool alwaysOnTop;
    private bool downloadToSubsFolder;
    private int foreignPartsSelectedIndex;
    private int hearingImpairedSelectedIndex;
    private bool includeAiTranslated;
    private bool includeMachineTranslated;
    private bool isLoggedIn;
    private string loginErrorText;
    private bool onlyFromTrustedSources;
    private bool overwriteSameLanguageSubs;
    private string password;
    private string resetTimer;
    private string searchText;
    private string selectedFormat;
    private SubtitleLanguage selectedLanguage;
    private SubtitleLanguage selectedWantedLanguage;

    private User user;
    private string username;

    public SettingsViewModel(INavigator navigator, IOpenSubtitlesService openSubtitlesService, ApplicationSettings settings, IEnumerable<SubtitleLanguage> allLanguages, IEnumerable<string> formats)
    {
        this.navigator = navigator;
        _openSubtitlesService = openSubtitlesService;
        _settings = settings;
        Formats = formats;
        SelectedFormat = _settings.PreferredFormat;
        this.allLanguages = allLanguages.ToList();
        foreach (var x in this.allLanguages)
        {
            LanguageList.Add(x);
        }

        if (_settings.WantedLanguages != null)
        {
            foreach (var x in _settings.WantedLanguageCodes)
            {
                WantedLanguageList.Add(allLanguages.SingleOrDefault(l => l.Code == x));
            }
        }

        alwaysOnTop = _settings.KeepWindowOnTop;
        downloadToSubsFolder = _settings.DownloadToSubsFolder;
        allowMultipleDownloads = _settings.AllowMultipleDownloads;
        overwriteSameLanguageSubs = _settings.OverwriteSameLanguageSub;
        includeAiTranslated = !_settings.DefaultSearchParameters.IncludeAiTranslated.HasValue || _settings.DefaultSearchParameters.IncludeAiTranslated == true;
        includeMachineTranslated = _settings.DefaultSearchParameters.IncludeMachineTranslated == true;
        onlyFromTrustedSources = _settings.DefaultSearchParameters.IncludeOnlyFromTrustedSources == true;

        if (_settings.DefaultSearchParameters.HearingImpaired.HasValue)
        {
            hearingImpairedSelectedIndex = (int)_settings.DefaultSearchParameters.HearingImpaired.Value;
        }

        if (_settings.DefaultSearchParameters.ForeignPartsOnly.HasValue)
        {
            foreignPartsSelectedIndex = (int)_settings.DefaultSearchParameters.ForeignPartsOnly.Value;
        }

        User = _settings.LoggedInUser;
        Username = _settings.LoggedInUser?.Username;
        IsLoggedIn = _settings.LoggedInUser != null;

        if (IsLoggedIn && User.ResetTime.HasValue && User.ResetTime.Value > DateTime.UtcNow)
        {
            var timeSpan = User.ResetTime.Value - DateTime.UtcNow;
            ResetTimer = $"{timeSpan.Hours:D2} hours and {timeSpan.Minutes:D2} minutes";
        }
    }

    public ICommand AddCommand => new RelayCommand(Add);

    public bool AllowMultipleDownloads
    {
        get => allowMultipleDownloads;

        set
        {
            Set(() => AllowMultipleDownloads, ref allowMultipleDownloads, value);
            Set(() => DownloadToSubsFolder, ref downloadToSubsFolder, false);
            Set(() => OverwriteSameLanguageSubs, ref overwriteSameLanguageSubs, false);
        }
    }

    public bool AlwaysOnTop
    {
        get => alwaysOnTop;
        set => Set(() => AlwaysOnTop, ref alwaysOnTop, value);
    }

    public ICommand CancelCommand => new RelayCommand(Cancel);

    public ICommand DeleteCommand => new RelayCommand(Delete);

    public bool DownloadToSubsFolder
    {
        get => downloadToSubsFolder;

        set => Set(() => DownloadToSubsFolder, ref downloadToSubsFolder, value);
    }

    public int ForeignPartsSelectedIndex
    {
        get => foreignPartsSelectedIndex;
        set => Set(() => ForeignPartsSelectedIndex, ref foreignPartsSelectedIndex, value);
    }

    public IEnumerable<string> Formats { get; }

    public int HearingImpairedSelectedIndex
    {
        get => hearingImpairedSelectedIndex;
        set => Set(() => HearingImpairedSelectedIndex, ref hearingImpairedSelectedIndex, value);
    }

    public bool IncludeAiTranslated
    {
        get => includeAiTranslated;
        set => Set(() => IncludeAiTranslated, ref includeAiTranslated, value);
    }

    public bool IncludeMachineTranslated
    {
        get => includeMachineTranslated;
        set => Set(() => IncludeMachineTranslated, ref includeMachineTranslated, value);
    }

    public bool IsLanguageSelected => SelectedLanguage != null;

    public bool IsLoggedIn
    {
        get => isLoggedIn;

        set => Set(() => IsLoggedIn, ref isLoggedIn, value);
    }

    public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;

    public ObservableCollection<SubtitleLanguage> LanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

    public ICommand LoginCommand => new RelayCommand(Login);

    public string LoginErrorText
    {
        get => loginErrorText;
        set => Set(() => LoginErrorText, ref loginErrorText, value);
    }

    public ICommand LogoutCommand => new RelayCommand(Logout);

    public bool OnlyFromTrustedSources
    {
        get => onlyFromTrustedSources;
        set => Set(() => OnlyFromTrustedSources, ref onlyFromTrustedSources, value);
    }

    public bool OverwriteSameLanguageSubs
    {
        get => overwriteSameLanguageSubs;

        set => Set(() => OverwriteSameLanguageSubs, ref overwriteSameLanguageSubs, value);
    }

    public string Password
    {
        get => password;
        set => Set(() => Password, ref password, value);
    }

    public string ResetTimer
    {
        get => resetTimer;
        set => Set(() => ResetTimer, ref resetTimer, value);
    }

    public ICommand SaveCommand => new RelayCommand(SaveAndBack);

    public string SearchText
    {
        get => searchText;

        set
        {
            searchText = value;
            LanguageList.Clear();
            foreach (var x in allLanguages)
            {
                if (x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Any(w => w.Code == x.Code))
                {
                    LanguageList.Add(x);
                }
            }

            Set(() => SearchText, ref searchText, value);
        }
    }

    public string SelectedFormat
    {
        get => selectedFormat;
        set => Set(() => SelectedFormat, ref selectedFormat, value);
    }

    public SubtitleLanguage SelectedLanguage
    {
        get => selectedLanguage;

        set
        {
            if (value != null && SelectedWantedLanguage != null)
            {
                SelectedWantedLanguage = null;
            }

            Set(() => SelectedLanguage, ref selectedLanguage, value);
            RaisePropertyChanged(() => IsLanguageSelected);
        }
    }

    public SubtitleLanguage SelectedWantedLanguage
    {
        get => selectedWantedLanguage;

        set
        {
            if (value != null && SelectedLanguage != null)
            {
                SelectedLanguage = null;
            }

            Set(() => SelectedWantedLanguage, ref selectedWantedLanguage, value);
            RaisePropertyChanged(() => IsWantedLanguageSelected);
        }
    }

    public User User
    {
        get => user;

        set => Set(() => User, ref user, value);
    }

    public string Username { get => username; set => Set(() => Username, ref username, value); }

    public ObservableCollection<SubtitleLanguage> WantedLanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

    private void Add()
    {
        while (SelectedLanguage != null)
        {
            var selected = SelectedLanguage;
            LanguageList.Remove(selected);
            WantedLanguageList.Add(selected);
        }
    }

    private void Cancel()
    {
        navigator.GoToPreviousControl();
    }

    private void Delete()
    {
        while (SelectedWantedLanguage != null)
        {
            var selected = SelectedWantedLanguage;
            WantedLanguageList.Remove(selected);
            LanguageList.Add(selected);
        }
    }

    private async void Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            LoginErrorText = "Enter your credentials.";
        }

        try
        {
            var result = await _openSubtitlesService.LoginAsync(Username, Password);

            IsLoggedIn = true;
            _settings.LoggedInUser = result;
            Password = null;
            User = result;
            _ = SettingsParser.SaveAsync(_settings);
        }
        catch (RequestFailedException ex)
        {
            LoginErrorText = ex.Message;
        }
    }

    private async void Logout()
    {
        if (await _openSubtitlesService.LogoutAsync())
        {
            _settings.LoggedInUser = null;
            IsLoggedIn = false;
            User = null;
            _ = SettingsParser.SaveAsync(_settings);
        }
    }

    private void SaveAndBack()
    {
        _settings.KeepWindowOnTop = alwaysOnTop;
        _settings.AllowMultipleDownloads = allowMultipleDownloads;
        _settings.DownloadToSubsFolder = downloadToSubsFolder;
        _settings.OverwriteSameLanguageSub = overwriteSameLanguageSubs;
        _settings.WantedLanguages = WantedLanguageList.ToList();
        _settings.DefaultSearchParameters.ForeignPartsOnly = (Filter)ForeignPartsSelectedIndex;
        _settings.DefaultSearchParameters.HearingImpaired = (Filter)HearingImpairedSelectedIndex;
        _settings.DefaultSearchParameters.IncludeOnlyFromTrustedSources = OnlyFromTrustedSources;
        _settings.DefaultSearchParameters.IncludeAiTranslated = IncludeAiTranslated;
        _settings.DefaultSearchParameters.IncludeMachineTranslated = IncludeMachineTranslated;
        _settings.PreferredFormat = SelectedFormat;
        _ = SettingsParser.SaveAsync(_settings);
        navigator.GoToPreviousControl();
    }
}
