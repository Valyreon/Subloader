using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using OpenSubtitlesSharp;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private readonly INavigator navigator;
    private bool allowMultipleDownloads;
    private bool alwaysOnTop;
    private bool downloadToSubsFolder;
    private bool overwriteSameLanguageSubs;
    private string searchText;
    private SubtitleLanguage selectedLanguage;
    private SubtitleLanguage selectedWantedLanguage;

    public SettingsViewModel(INavigator navigator)
    {
        this.navigator = navigator;
        var wantLangs = App.Settings.WantedLanguages;
        foreach (var x in App.Settings.AllLanguages)
        {
            LanguageList.Add(x);
        }

        if (wantLangs != null)
        {
            foreach (var x in wantLangs)
            {
                WantedLanguageList.Add(App.Settings.AllLanguages.Single(c => c.Code == x));
            }
        }

        alwaysOnTop = App.Settings.KeepWindowOnTop;
        downloadToSubsFolder = App.Settings.DownloadToSubsFolder;
        allowMultipleDownloads = App.Settings.AllowMultipleDownloads;
        overwriteSameLanguageSubs = App.Settings.OverwriteSameLanguageSub;
    }

    public ICommand AddCommand => new RelayCommand(Add);

    public bool AllowMultipleDownloads
    {
        get => allowMultipleDownloads;

        set
        {
            Set(nameof(AllowMultipleDownloads), ref allowMultipleDownloads, value);
            Set(nameof(DownloadToSubsFolder), ref downloadToSubsFolder, false);
            Set(nameof(OverwriteSameLanguageSubs), ref overwriteSameLanguageSubs, false);
        }
    }

    public bool AlwaysOnTop
    {
        get => alwaysOnTop;
        set => Set(nameof(AlwaysOnTop), ref alwaysOnTop, value);
    }

    public ICommand CancelCommand => new RelayCommand(Cancel);
    public ICommand DeleteCommand => new RelayCommand(Delete);

    public bool DownloadToSubsFolder
    {
        get => downloadToSubsFolder;

        set => Set(nameof(DownloadToSubsFolder), ref downloadToSubsFolder, value);
    }

    public bool IsLanguageSelected => SelectedLanguage != null;
    public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;
    public ObservableCollection<SubtitleLanguage> LanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

    public bool OverwriteSameLanguageSubs
    {
        get => overwriteSameLanguageSubs;

        set => Set(nameof(OverwriteSameLanguageSubs), ref overwriteSameLanguageSubs, value);
    }

    public ICommand SaveCommand => new RelayCommand(SaveAndBack);

    public string SearchText
    {
        get => searchText;

        set
        {
            searchText = value;
            LanguageList.Clear();
            foreach (var x in App.Settings.AllLanguages)
            {
                if (x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Any(w => w.Code == x.Code))
                {
                    LanguageList.Add(x);
                }
            }

            Set(nameof(SearchText), ref searchText, value);
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

            Set(nameof(SelectedLanguage), ref selectedLanguage, value);
            RaisePropertyChanged(nameof(IsLanguageSelected));
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

            Set(nameof(SelectedWantedLanguage), ref selectedWantedLanguage, value);
            RaisePropertyChanged(nameof(IsWantedLanguageSelected));
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
        var wanted = new List<SubtitleLanguage>();
        wanted.AddRange(WantedLanguageList);

        App.Settings.KeepWindowOnTop = alwaysOnTop;
        App.Settings.AllowMultipleDownloads = allowMultipleDownloads;
        App.Settings.DownloadToSubsFolder = downloadToSubsFolder;
        App.Settings.OverwriteSameLanguageSub = overwriteSameLanguageSubs;
        App.Settings.WantedLanguages = wanted.Select(w => w.Code).ToList();
        SettingsParser.Save(App.Settings);
        navigator.GoToPreviousControl();
    }
}
