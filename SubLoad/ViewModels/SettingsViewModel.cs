using SubLoad.Models;
using SubLoad.Views;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SubLoad.ViewModels
{
    public class SettingsViewModel: ObservableObject
    {
        private IView currentWindow;
        public ObservableCollection<SubtitleLanguage> LanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();
        public ObservableCollection<SubtitleLanguage> WantedLanguageList { get; set; } = new ObservableCollection<SubtitleLanguage>();

        private SubtitleLanguage selectedWantedLanguage;
        public SubtitleLanguage SelectedWantedLanguage
        {
            get
            {
                return selectedWantedLanguage;
            }

            set
            {
                if (value != null && SelectedLanguage != null)
                    SelectedLanguage = null;
                selectedWantedLanguage = value;
                RaisePropertyChangedEvent("SelectedWantedLanguage");
                RaisePropertyChangedEvent("IsWantedLanguageSelected");
            }
        }

        private SubtitleLanguage selectedLanguage;
        public SubtitleLanguage SelectedLanguage
        {
            get
            {
                return selectedLanguage;
            }

            set
            {
                if (value != null && SelectedWantedLanguage != null)
                    SelectedWantedLanguage = null;
                selectedLanguage = value;
                RaisePropertyChangedEvent("SelectedLanguage");
                RaisePropertyChangedEvent("IsLanguageSelected");
            }
        }

        public bool IsLanguageSelected
        {
            get => SelectedLanguage != null;
            set { }
        }

        public bool IsWantedLanguageSelected
        {
            get => SelectedWantedLanguage != null;
            set { }
        }

        private string searchText;
        public string SearchText
        {
            get
            {
                return searchText;
            }

            set
            {
                searchText = value;
                LanguageList.Clear();
                foreach(var x in SubtitleLanguage.AllLanguages)
                {
                    if(x.Name.ToLower().Contains(searchText == null ? string.Empty : searchText.ToLower()) && !WantedLanguageList.Contains(x))
                    {
                        LanguageList.Add(x);
                    }
                }
                RaisePropertyChangedEvent("SearchText");
            }
        }

        public SettingsViewModel(IView thisWindow, IEnumerable<SubtitleLanguage> wantLangs)
        {
            this.currentWindow = thisWindow;
            foreach(var x in SubtitleLanguage.AllLanguages)
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
        }

        public ICommand AddCommand { get => new DelegateCommand(Add); }
        public ICommand DeleteCommand { get => new DelegateCommand(Delete); }
        public ICommand SaveCommand { get => new DelegateCommand(SaveAndBack); }

        public void Add()
        {
            if (SelectedLanguage != null)
            {
                var selected = SelectedLanguage;
                LanguageList.Remove(selected);
                WantedLanguageList.Add(selected);
            }
        }

        public void Delete()
        {
            if (SelectedWantedLanguage != null)
            {
                var selected = SelectedWantedLanguage;
                WantedLanguageList.Remove(selected);
                LanguageList.Add(selected);
                SearchText = SearchText;
            }
        }

        public void SaveAndBack()
        {
            ApplicationSettings.WriteApplicationSettings(WantedLanguageList);
            this.currentWindow.GoToPreviousControl();
        }
    }
}
