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
using SubloaderWpf.Views;
using System.Resources;
using System.Text.Json;
using System.Diagnostics;
using SubloaderWpf.Services;
using System.Windows;

namespace SubloaderWpf.ViewModels;

public class SettingsViewModel : ObservableEntity
{
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
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
    private ObservableCollection<SubtitleLanguage> languageList = new();
    private bool isLoggingIn;
    private bool isLoggingOut;
    private bool isCheckingForUpdates;

    public static IEnumerable<SubtitleLanguage> AllLanguages { get; set; }

    static SettingsViewModel()
    {
        var manager = new ResourceManager("SubloaderWpf.Resources.Resources", typeof(TheWindow).Assembly);
        AllLanguages = JsonSerializer.Deserialize<IEnumerable<SubtitleLanguage>>(manager.GetString("LanguagesList"));
    }

    public SettingsViewModel(INavigator navigator, IOpenSubtitlesService openSubtitlesService, ApplicationSettings settings)
    {
        this.navigator = navigator;
        _openSubtitlesService = openSubtitlesService;
        _settings = settings;
        SelectedFormat = _settings.PreferredFormat;

        Formats = new List<string> { "srt", "sub", "mpl", "webvtt", "dfxp", "txt" };

        if (_settings.WantedLanguages?.Any() == true)
        {
            WantedLanguageList = new ObservableCollection<SubtitleLanguage>(_settings.WantedLanguages.Select(x => AllLanguages.SingleOrDefault(l => l.Code == x)));
            LanguageList = new ObservableCollection<SubtitleLanguage>(AllLanguages.Except(WantedLanguageList));
        }
        else
        {
            LanguageList = new ObservableCollection<SubtitleLanguage>(AllLanguages);
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

        PropertyChanged += (e, v) => Save();
        WantedLanguageList.CollectionChanged += (e, v) => Save();
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

    public bool IsLoggingIn
    {
        get => isLoggingIn;
        set => Set(() => IsLoggingIn, ref isLoggingIn, value);
    }

    public bool IsLoggingOut
    {
        get => isLoggingOut;
        set => Set(() => IsLoggingOut, ref isLoggingOut, value);
    }

    public ICommand BackCommand => new RelayCommand(() => navigator.GoToPreviousControl());

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

    public bool IsCheckingForUpdates
    {
        get => isCheckingForUpdates;
        set => Set(() => IsCheckingForUpdates, ref isCheckingForUpdates, value);
    }

    public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;

    public ObservableCollection<SubtitleLanguage> LanguageList
    {
        get => languageList;
        set => Set(() => LanguageList, ref languageList, value);
    }

    public ICommand LoginCommand => new RelayCommand(Login);

    public ICommand CheckForUpdatesCommand => new RelayCommand(CheckForUpdates);

    private async void CheckForUpdates()
    {
        var service = new GitHubService();

        try
        {
            IsCheckingForUpdates = true;
            var isLatestVersion = await service.IsLatestVersionAsync(App.VersionTag);
            IsCheckingForUpdates = false;

            if (isLatestVersion)
            {
                MessageBox.Show("You have the latest version!", "Update Check");
            }
            else
            {
                var result = MessageBox.Show("New version of Subloader is available. Do you want to download now?", "Update available", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/Valyreon/Subloader/releases/latest") { UseShellExecute = true });
                }
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Something went wrong while checking for updates, please try again later.");
        }
    }

    public ICommand OpenProjectHomeCommand => new RelayCommand(() => Process.Start(new ProcessStartInfo("https://github.com/Valyreon/Subloader") { UseShellExecute = true }));

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

    public string SearchText
    {
        get => searchText;

        set
        {
            searchText = value;
            LanguageList = new(AllLanguages.Where(x => x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Any(w => w.Code == x.Code)));
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

    public void Add()
    {
        while (SelectedLanguage != null)
        {
            var selected = SelectedLanguage;
            LanguageList.Remove(selected);

            var index = 0;
            while (index < WantedLanguageList.Count && string.CompareOrdinal(selected.Name, WantedLanguageList[index].Name) > 0)
            {
                index++;
            }

            WantedLanguageList.Insert(index, selected);

            if (!LanguageList.Any())
            {
                SearchText = string.Empty;
            }
        }
    }

    public void Delete()
    {
        while (SelectedWantedLanguage != null)
        {
            var selected = SelectedWantedLanguage;
            WantedLanguageList.Remove(selected);

            var index = 0;
            while (index < LanguageList.Count && string.CompareOrdinal(selected.Name, LanguageList[index].Name) > 0)
            {
                index++;
            }

            LanguageList.Insert(index, selected);
        }
    }

    private async void Login()
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            LoginErrorText = "Enter your credentials.";
            return;
        }

        LoginErrorText = null;
        IsLoggingIn = true;

        try
        {
            var result = await _openSubtitlesService.LoginAsync(Username, Password);

            IsLoggedIn = true;
            _settings.LoggedInUser = result;
            Password = null;
            User = result;
            _ = ApplicationDataReader.SaveSettingsAsync(_settings);
        }
        catch (RequestFailedException ex)
        {
            LoginErrorText = ex.Message;
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    private async void Logout()
    {
        IsLoggingOut = true;
        if (await _openSubtitlesService.LogoutAsync())
        {
            _settings.LoggedInUser = null;
            IsLoggedIn = false;
            User = null;
            _ = ApplicationDataReader.SaveSettingsAsync(_settings);
        }
        IsLoggingOut = false;
    }

    private void Save()
    {
        _settings.KeepWindowOnTop = alwaysOnTop;
        _settings.AllowMultipleDownloads = allowMultipleDownloads;
        _settings.DownloadToSubsFolder = downloadToSubsFolder;
        _settings.OverwriteSameLanguageSub = overwriteSameLanguageSubs;
        _settings.WantedLanguages = WantedLanguageList.Select(l => l.Code).ToList();
        _settings.DefaultSearchParameters.ForeignPartsOnly = (Filter)ForeignPartsSelectedIndex;
        _settings.DefaultSearchParameters.HearingImpaired = (Filter)HearingImpairedSelectedIndex;
        _settings.DefaultSearchParameters.IncludeOnlyFromTrustedSources = OnlyFromTrustedSources;
        _settings.DefaultSearchParameters.IncludeAiTranslated = IncludeAiTranslated;
        _settings.DefaultSearchParameters.IncludeMachineTranslated = IncludeMachineTranslated;
        _settings.PreferredFormat = SelectedFormat;
        _ = ApplicationDataReader.SaveSettingsAsync(_settings);
    }
}
