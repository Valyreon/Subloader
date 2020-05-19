﻿using SubloaderWpf.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace SubloaderWpf.ViewModels
{
    public class SettingsViewModel: ViewModelBase
    {
        private readonly INavigator navigator;
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
                    Set("SelectedLanguage", ref selectedLanguage, null);
                Set("SelectedWantedLanguage", ref selectedWantedLanguage, value);
                RaisePropertyChanged("IsWantedLanguageSelected");
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
                    Set("SelectedWantedLanguage", ref selectedWantedLanguage, null);
                Set("SelectedLanguage", ref selectedLanguage, value);
                RaisePropertyChanged("IsLanguageSelected");
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
                Set("SearchText", ref searchText, value);
            }
        }

        public SettingsViewModel(INavigator navigator)
        {
            this.navigator = navigator;
            var wantLangs = ApplicationSettings.GetInstance().WantedLanguages;
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
        }

        public ICommand AddCommand { get => new RelayCommand(Add); }
        public ICommand DeleteCommand { get => new RelayCommand(Delete); }
        public ICommand SaveCommand { get => new RelayCommand(SaveAndBack); }

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
                SelectedWantedLanguage = WantedLanguageList.Count == 0 ? null : WantedLanguageList[0];
            }
        }

        public void SaveAndBack()
        {
            List<SubtitleLanguage> wanted = new List<SubtitleLanguage>();
            foreach(var x in WantedLanguageList)
            {
                wanted.Add(x);
            }
            ApplicationSettings.GetInstance().WantedLanguages = wanted;
            navigator.GoToPreviousControl();
        }
    }
}