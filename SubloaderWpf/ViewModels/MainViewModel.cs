using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using SubloaderWpf.Models;
using SuppliersLibrary;
using SuppliersLibrary.OpenSubtitles;

namespace SubloaderWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private string statusText;
        private string currentPath;
        private bool searchByName;
        private bool searchByHash;

        private readonly List<ISubtitleSupplier> suppliers = new List<ISubtitleSupplier>();

        public MainViewModel(INavigator navigator)
        {
            this.navigator = navigator;
            // Must first add suppliers before processing.
            suppliers.Add(new OpenSubtitles());

            searchByHash = ApplicationSettings.GetInstance().IsByHashChecked;
            searchByName = ApplicationSettings.GetInstance().IsByNameChecked;

            StatusText = "Open a video file.";
            CurrentPath = (Application.Current as App).PathArg;
        }

        public ObservableCollection<SubtitleEntry> SubtitleList { get; set; } = new ObservableCollection<SubtitleEntry>();

        public SubtitleEntry SelectedItem { get; set; }

        public string CurrentPath
        {
            get => currentPath;
            set
            {
                currentPath = value;
                if (currentPath != null)
                {
                    ProcessFileAsync();
                }
            }
        }

        public string StatusText
        {
            get => statusText;

            set => Set("StatusText", ref statusText, value);
        }

        public bool SearchByName
        {
            get => searchByName;

            set
            {
                Set("SearchByName", ref searchByName, value);
                ApplicationSettings.GetInstance().IsByNameChecked = value;
            }
        }

        public bool SearchByHash
        {
            get => searchByHash;

            set
            {
                Set("SearchByHash", ref searchByHash, value);
                ApplicationSettings.GetInstance().IsByHashChecked = value;

            }
        }

        public ICommand ChooseFileCommand => new RelayCommand(ChooseFile);
        public ICommand RefreshCommand => new RelayCommand(Refresh);
        public ICommand SettingsCommand => new RelayCommand(GoToSettings);
        public ICommand DownloadCommand => new RelayCommand(Download);

        public void ChooseFile()
        {
            var fileChooseDialog = new OpenFileDialog
            {
                Filter = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ",
                CheckFileExists = true,
                CheckPathExists = true
            };
            _ = fileChooseDialog.ShowDialog();
            try
            {
                var fileInfo = new FileInfo(fileChooseDialog.FileName);
                SubtitleList.Clear();
                CurrentPath = fileChooseDialog.FileName;
            }
            catch (Exception)
            {

            }
        }

        public void GoToSettings()
        {
            var settingsControl = new SettingsViewModel(navigator);
            navigator.GoToControl(settingsControl);
        }

        public async void Refresh()
        {
            if (CurrentPath != null)
            {
                await Task.Run(() => ProcessFileAsync());
            }
        }

        public async void Download()
        {
            if (SelectedItem == null)
            {
                return;
            }

            try
            {
                StatusText = "Downloading...";
                await Task.Run(() => Thread.Sleep(20));
                SelectedItem.Model.Download(Path.ChangeExtension(CurrentPath, SelectedItem.Model.Format));
                StatusText = "Subtitle downloaded.";
            }
            catch (Exception)
            {
                StatusText = "Error while downloading.";
                SystemSounds.Hand.Play();
            }
        }

        private async void ProcessFileAsync()
        {
            try
            {
                StatusText = "Searching subtitles...";
                App.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
                var results = await SearchSuppliers();
                if (results == null)
                {
                    StatusText = "Server error. Try refreshing.";
                }
                else if (results.Count == 0)
                {
                    StatusText = "No subtitles found.";
                }
                else
                {
                    foreach (var x in results)
                    {
                        App.Current.Dispatcher.Invoke(() => SubtitleList.Add(x));
                        await Task.Run(() => Thread.Sleep(20));
                    }

                    if (SubtitleList.Count > 0)
                    {
                        StatusText = "Use button or doubleclick to download.";
                    }
                    else
                    {
                        StatusText = "No subtitles found.";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
                SystemSounds.Hand.Play();
            }
        }

        private async Task<List<SubtitleEntry>> SearchSuppliers()
        {
            var result = new List<SubtitleEntry>();
            foreach (var supplier in suppliers)
            {
                var results = await supplier.SearchAsync(currentPath, new object[] { SearchByHash, SearchByName });
                foreach (var item in results)
                {
                    var settings = ApplicationSettings.GetInstance();
                    if (settings.WantedLanguages == null ||
                        settings.WantedLanguages.Count == 0 ||
                        settings.WantedLanguages.Where((subLang) => subLang.Name == item.Language).Any())
                    {
                        result.Add(new SubtitleEntry(item));
                    }
                }
            }
            return result;
        }
    }
}
