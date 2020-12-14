using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;

namespace SubloaderWpf.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private SubtitleLanguage selectedWantedLanguage;
        private SubtitleLanguage selectedLanguage;
        private string searchText;
        private bool alwaysOnTop;
        private bool downloadToSubsFolder;
        private bool allowMultipleDownloads;

        public SettingsViewModel(INavigator navigator)
        {
            this.navigator = navigator;
            var wantLangs = ApplicationSettings.Instance.WantedLanguages;
            foreach (var x in SubtitleLanguage.AllLanguages)
            {
                LanguageList.Add(x);
            }

            if (wantLangs != null)
            {
                foreach (var x in wantLangs)
                {
                    WantedLanguageList.Add(x);
                }
            }

            SelectedLanguage = null;
            SelectedWantedLanguage = null;
            AlwaysOnTop = ApplicationSettings.Instance.KeepWindowOnTop;
            DownloadToSubsFolder = ApplicationSettings.Instance.DownloadToSubsFolder;
            AllowMultipleDownloads = ApplicationSettings.Instance.AllowMultipleDownloads;
        }

        public ObservableCollection<SubtitleLanguage> LanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

        public ObservableCollection<SubtitleLanguage> WantedLanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

        public SubtitleLanguage SelectedWantedLanguage
        {
            get => selectedWantedLanguage;

            set
            {
                if (value != null && SelectedLanguage != null)
                {
                    Set("SelectedLanguage", ref selectedLanguage, null);
                }

                Set("SelectedWantedLanguage", ref selectedWantedLanguage, value);
                RaisePropertyChanged("IsWantedLanguageSelected");
            }
        }

        public SubtitleLanguage SelectedLanguage
        {
            get => selectedLanguage;

            set
            {
                if (value != null && SelectedWantedLanguage != null)
                {
                    Set("SelectedWantedLanguage", ref selectedWantedLanguage, null);
                }

                Set("SelectedLanguage", ref selectedLanguage, value);
                RaisePropertyChanged("IsLanguageSelected");
            }
        }

        public bool IsLanguageSelected => SelectedLanguage != null;

        public bool IsWantedLanguageSelected => SelectedWantedLanguage != null;

        public bool DownloadToSubsFolder
        {
            get => downloadToSubsFolder;

            set => Set("DownloadToSubsFolder", ref downloadToSubsFolder, value);
        }

        public bool AllowMultipleDownloads
        {
            get => allowMultipleDownloads;

            set
            {
                Set("AllowMultipleDownloads", ref allowMultipleDownloads, value);
                Set("DownloadToSubsFolder", ref downloadToSubsFolder, false);
            }
        }

        public bool AlwaysOnTop
        {
            get => alwaysOnTop;
            set => Set("AlwaysOnTop", ref alwaysOnTop, value);
        }

        public string SearchText
        {
            get => searchText;

            set
            {
                searchText = value;
                LanguageList.Clear();
                foreach (var x in SubtitleLanguage.AllLanguages)
                {
                    if (x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Any(w => w.Code == x.Code))
                    {
                        LanguageList.Add(x);
                    }
                }

                Set("SearchText", ref searchText, value);
            }
        }

        public ICommand AddCommand => new RelayCommand(Add);

        public ICommand DeleteCommand => new RelayCommand(Delete);

        public ICommand SaveCommand => new RelayCommand(SaveAndBack);

        public ICommand CancelCommand => new RelayCommand(Cancel);

        private void Cancel()
        {
            navigator.GoToPreviousControl();
        }

        private void Add()
        {
            while (SelectedLanguage != null)
            {
                var selected = SelectedLanguage;
                LanguageList.Remove(selected);
                WantedLanguageList.Add(selected);
            }
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
            foreach (var x in WantedLanguageList)
            {
                wanted.Add(x);
            }

            ApplicationSettings.Instance.KeepWindowOnTop = alwaysOnTop;
            ApplicationSettings.Instance.AllowMultipleDownloads = allowMultipleDownloads;
            ApplicationSettings.Instance.DownloadToSubsFolder = downloadToSubsFolder;
            ApplicationSettings.Instance.WantedLanguages = wanted;
            ApplicationSettings.Instance.Save();
            navigator.GoToPreviousControl();
        }
    }
}
