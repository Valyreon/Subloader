using SubLoad.Models;
using SubLoad.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                selectedWantedLanguage = value;
                RaisePropertyChangedEvent("SelectedWantedLanguage");
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
                selectedLanguage = value;
                RaisePropertyChangedEvent("SelectedLanguage");
            }
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
                    if(x.Name.ToLower().Contains(searchText.ToLower()) && !WantedLanguageList.Contains(x))
                    {
                        LanguageList.Add(x);
                    }
                }
                RaisePropertyChangedEvent("SearchText");
            }
        }

        public SettingsViewModel(IView thisWindow)
        {
            this.currentWindow = thisWindow;
            foreach(var x in SubtitleLanguage.AllLanguages)
            {
                LanguageList.Add(x);
            }
            SelectedLanguage = LanguageList[0];
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
            // save here
            this.currentWindow.GoToPreviousControl();
        }
    }
}
