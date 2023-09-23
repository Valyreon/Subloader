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
    private readonly ApplicationSettings _settings;
    private readonly IEnumerable<SubtitleLanguage> allLanguages;
    private readonly INavigator navigator;
    private bool allowMultipleDownloads;
    private bool alwaysOnTop;
    private bool downloadToSubsFolder;
    private bool overwriteSameLanguageSubs;
    private string searchText;
    private SubtitleLanguage selectedLanguage;
    private SubtitleLanguage selectedWantedLanguage;

    public SettingsViewModel(INavigator navigator, ApplicationSettings settings, IEnumerable<SubtitleLanguage> allLanguages)
    {
        this.navigator = navigator;
        _settings = settings;
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

    public bool IsLanguageSelected => SelectedLanguage != null;
    public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;
    public ObservableCollection<SubtitleLanguage> LanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

    public bool OverwriteSameLanguageSubs
    {
        get => overwriteSameLanguageSubs;

        set => Set(() => OverwriteSameLanguageSubs, ref overwriteSameLanguageSubs, value);
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
            SearchText = SearchText;
        }
    }

    private void SaveAndBack()
    {
        _settings.KeepWindowOnTop = alwaysOnTop;
        _settings.AllowMultipleDownloads = allowMultipleDownloads;
        _settings.DownloadToSubsFolder = downloadToSubsFolder;
        _settings.OverwriteSameLanguageSub = overwriteSameLanguageSubs;
        _settings.WantedLanguages = WantedLanguageList.ToList();
        _ = SettingsParser.SaveAsync(_settings);
        navigator.GoToPreviousControl();
    }
}
