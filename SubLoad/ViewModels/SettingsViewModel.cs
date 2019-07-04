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

        public SettingsViewModel(IView thisWindow)
        {
            this.currentWindow = thisWindow;
            LanguageList.Add(new SubtitleLanguage(1, "English", "eng"));
            LanguageList.Add(new SubtitleLanguage(2, "Serbian", "srb"));
            LanguageList.Add(new SubtitleLanguage(3, "Croatian", "cro"));
            SelectedLanguage = LanguageList[0];
        }

        public ICommand AddCommand { get => new DelegateCommand(Add); }
        public ICommand DeleteCommand { get => new DelegateCommand(Delete); }
        public ICommand SaveCommand { get => new DelegateCommand(SaveAndBack); }

        public void Add()
        {
            if(!WantedLanguageList.Where((x)=> x.Name == this.SelectedLanguage.Name).Any())
            {
                WantedLanguageList.Add(this.SelectedLanguage);
            }
        }

        public void Delete()
        {
            WantedLanguageList.Remove(this.SelectedWantedLanguage);
            this.SelectedWantedLanguage = null;
        }

        public void SaveAndBack()
        {
            // save here
            this.currentWindow.GoToPreviousControl();
        }
    }
}
