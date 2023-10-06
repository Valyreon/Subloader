using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenSubtitlesSharp;
using System.Resources;
using System.Text.Json;
using System.Diagnostics;
using SubloaderAvalonia.Interfaces;
using SubloaderAvalonia.Models;
using SubloaderAvalonia.Views;
using ReactiveUI;
using System.Reactive;
using SubloaderAvalonia.Utilities;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using SubloaderAvalonia.Services;

namespace SubloaderAvalonia.ViewModels;

public class SettingsViewModel : ViewModelBase
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

    public static IReadOnlyDictionary<string, SubtitleLanguage> AllLanguages { get; set; }

    static SettingsViewModel()
    {
        var manager = new ResourceManager("SubloaderAvalonia.Resources.Resources", typeof(MainWindow).Assembly);
        AllLanguages = JsonSerializer.Deserialize<IEnumerable<SubtitleLanguage>>(manager.GetString("LanguagesList")).ToDictionary(x => x.Code);
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
            WantedLanguageList = new ObservableCollection<SubtitleLanguage>(_settings.WantedLanguages.Select(x => AllLanguages[x]));
            LanguageList = new ObservableCollection<SubtitleLanguage>(AllLanguages.Values.Except(WantedLanguageList));
        }
        else
        {
            LanguageList = new ObservableCollection<SubtitleLanguage>(AllLanguages.Values);
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

    public ReactiveCommand<Unit, Unit> AddCommand => ReactiveCommand.Create(Add);

    public bool AllowMultipleDownloads
    {
        get => allowMultipleDownloads;

        set
        {
            this.RaiseAndSetIfChanged(ref allowMultipleDownloads, value);
            this.RaiseAndSetIfChanged(ref downloadToSubsFolder, false);
            this.RaiseAndSetIfChanged(ref overwriteSameLanguageSubs, false);
        }
    }

    public bool AlwaysOnTop
    {
        get => alwaysOnTop;
        set => this.RaiseAndSetIfChanged(ref alwaysOnTop, value);
    }

    public bool IsLoggingIn
    {
        get => isLoggingIn;
        set => this.RaiseAndSetIfChanged(ref isLoggingIn, value);
    }

    public bool IsLoggingOut
    {
        get => isLoggingOut;
        set => this.RaiseAndSetIfChanged(ref isLoggingOut, value);
    }

    public ReactiveCommand<Unit, Unit> BackCommand => ReactiveCommand.Create(() => navigator.GoToPreviousControl());

    public ReactiveCommand<Unit, Unit> DeleteCommand => ReactiveCommand.Create(Delete);

    public bool DownloadToSubsFolder
    {
        get => downloadToSubsFolder;

        set => this.RaiseAndSetIfChanged(ref downloadToSubsFolder, value);
    }

    public int ForeignPartsSelectedIndex
    {
        get => foreignPartsSelectedIndex;
        set => this.RaiseAndSetIfChanged(ref foreignPartsSelectedIndex, value);
    }

    public IEnumerable<string> Formats { get; }

    public int HearingImpairedSelectedIndex
    {
        get => hearingImpairedSelectedIndex;
        set => this.RaiseAndSetIfChanged(ref hearingImpairedSelectedIndex, value);
    }

    public bool IncludeAiTranslated
    {
        get => includeAiTranslated;
        set => this.RaiseAndSetIfChanged(ref includeAiTranslated, value);
    }

    public bool IncludeMachineTranslated
    {
        get => includeMachineTranslated;
        set => this.RaiseAndSetIfChanged(ref includeMachineTranslated, value);
    }

    public bool IsLanguageSelected => SelectedLanguage != null;

    public bool IsLoggedIn
    {
        get => isLoggedIn;
        set => this.RaiseAndSetIfChanged(ref isLoggedIn, value);
    }

    public bool IsCheckingForUpdates
    {
        get => isCheckingForUpdates;
        set => this.RaiseAndSetIfChanged(ref isCheckingForUpdates, value);
    }

    public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;

    public ObservableCollection<SubtitleLanguage> LanguageList
    {
        get => languageList;
        set => this.RaiseAndSetIfChanged(ref languageList, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand => ReactiveCommand.Create(Login);

    public ReactiveCommand<Unit, Unit> CheckForUpdatesCommand => ReactiveCommand.Create(CheckForUpdates);

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
                var box = MessageBoxManager.GetMessageBoxStandard("Update Check", "You have the latest version!", ButtonEnum.Ok);
                await box.ShowAsync();
            }
            else
            {
                var box = MessageBoxManager.GetMessageBoxStandard(
                    "Update available",
                    "New version of Subloader is available. Do you want to download now?",
                    ButtonEnum.YesNo);
                var result = await box.ShowAsync();

                if (result == ButtonResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/Valyreon/Subloader/releases/latest") { UseShellExecute = true });
                }
            }
        }
        catch (Exception)
        {
            var box = MessageBoxManager.GetMessageBoxStandard("Update Check", "Something went wrong while checking for updates, please try again later.", ButtonEnum.Ok);
            await box.ShowAsync();
        }
    }

    public ReactiveCommand<Unit, Process> OpenProjectHomeCommand => ReactiveCommand.Create(() => Process.Start(new ProcessStartInfo("https://github.com/Valyreon/Subloader") { UseShellExecute = true }));

    public string LoginErrorText
    {
        get => loginErrorText;
        set => this.RaiseAndSetIfChanged(ref loginErrorText, value);
    }

    public ReactiveCommand<Unit, Unit> LogoutCommand => ReactiveCommand.Create(Logout);

    public bool OnlyFromTrustedSources
    {
        get => onlyFromTrustedSources;
        set => this.RaiseAndSetIfChanged(ref onlyFromTrustedSources, value);
    }

    public bool OverwriteSameLanguageSubs
    {
        get => overwriteSameLanguageSubs;

        set => this.RaiseAndSetIfChanged(ref overwriteSameLanguageSubs, value);
    }

    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public string ResetTimer
    {
        get => resetTimer;
        set => this.RaiseAndSetIfChanged(ref resetTimer, value);
    }

    public string SearchText
    {
        get => searchText;

        set
        {
            searchText = value;
            LanguageList = new(AllLanguages.Values.Where(x => x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Any(w => w.Code == x.Code)));
            this.RaiseAndSetIfChanged(ref searchText, value);
        }
    }

    public string SelectedFormat
    {
        get => selectedFormat;
        set => this.RaiseAndSetIfChanged(ref selectedFormat, value);
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

            this.RaiseAndSetIfChanged(ref selectedLanguage, value);
            this.RaisePropertyChanged(nameof(IsLanguageSelected));
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

            this.RaiseAndSetIfChanged(ref selectedWantedLanguage, value);
            this.RaisePropertyChanged(nameof(IsWantedLanguageSelected));
        }
    }

    public User User
    {
        get => user;

        set => this.RaiseAndSetIfChanged(ref user, value);
    }

    public string Username { get => username; set => this.RaiseAndSetIfChanged(ref username, value); }

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
