using Microsoft.Win32;
using SubloaderWpf.Models;
using SuppliersLibrary;
using SuppliersLibrary.OpenSubtitles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SubloaderWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INavigator navigator;
        private string statusText;
        private string currentPath;

        private readonly List<ISubtitleSupplier> suppliers = new List<ISubtitleSupplier>();

        public MainViewModel(INavigator navigator)
        {
            this.navigator = navigator;
            // Must first add suppliers before processing.
            suppliers.Add(new OpenSubtitles());

            StatusText = "Open a video file.";
            CurrentPath = (Application.Current as App).PathArg;
        }

        public ObservableCollection<SubtitleEntry> SubtitleList { get; set; } = new ObservableCollection<SubtitleEntry>();

        public SubtitleEntry SelectedItem { get; set; }

        public string CurrentPath
        {
            get { return currentPath; }
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
            get
            {
                return statusText;
            }

            set
            {
                Set("StatusText", ref statusText, value);
            }
        }

        public ICommand ChooseFileCommand { get => new RelayCommand(ChooseFile); }
        public ICommand RefreshCommand { get => new RelayCommand(Refresh); }
        public ICommand SettingsCommand { get => new RelayCommand(GoToSettings); }
        public ICommand DownloadCommand { get => new RelayCommand(Download); }

        public void ChooseFile()
        {
            OpenFileDialog fileChooseDialog = new OpenFileDialog
            {
                Filter = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; *.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; *.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; *.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ",
                CheckFileExists = true,
                CheckPathExists = true
            };
            fileChooseDialog.ShowDialog();
            try
            {
                FileInfo fileInfo = new FileInfo(fileChooseDialog.FileName);
                this.SubtitleList.Clear();
                CurrentPath = fileChooseDialog.FileName;
            }
            catch (Exception)
            {
                this.StatusText = "Open a video file.";
            }
        }

        public void GoToSettings()
        {
            SettingsViewModel settingsControl = new SettingsViewModel(navigator);
            navigator.GoToControl(settingsControl);
        }

        public async void Refresh()
        {
            if (this.CurrentPath != null)
            {
                await Task.Run(() => this.ProcessFileAsync());
            }
        }

        public void Download()
        {
            if (SelectedItem == null)
                return;
            try
            {
                this.StatusText = "Downloading...";
                SelectedItem.Model.Download(Path.ChangeExtension(this.CurrentPath, SelectedItem.Model.Format));
                this.StatusText = "Subtitle downloaded.";
            }
            catch (Exception)
            {
                //this.StatusText = ex.Message;//"Error while downloading. Try again.";
                this.StatusText = "Error while downloading.";
            }
        }

        private async void ProcessFileAsync()
        {
            try
            {
                StatusText = "Searching subtitles...";
                App.Current.Dispatcher.Invoke(() => this.SubtitleList.Clear());
                var results = await this.SearchSuppliers();
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
                        App.Current.Dispatcher.Invoke(() => this.SubtitleList.Add(x));
                        await Task.Run(() => Thread.Sleep(20));
                    }
                    if (this.SubtitleList.Count > 0)
                    {
                        this.StatusText = "Select a subtitle and click Download.";
                    }
                    else
                    {
                        this.StatusText = "No subtitles found.";
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
            }
        }

        private async Task<List<SubtitleEntry>> SearchSuppliers()
        {
            List<SubtitleEntry> result = new List<SubtitleEntry>();
            foreach(var supplier in suppliers)
            {
                var results = await supplier.SearchAsync(currentPath);
                foreach(var item in results)
                {
                    var settings = ApplicationSettings.GetInstance();
                    if (settings.WantedLanguages == null || settings.WantedLanguages.Count == 0 || settings.WantedLanguages.Where((subLang) => subLang.Name == item.Language).Any())
                    {
                        result.Add(new SubtitleEntry(item));
                    }
                }
            }
            return result;
        }
    }
}
