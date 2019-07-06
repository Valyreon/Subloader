using SubLib;
using SubLoad.Models;
using SubLoad.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SubLoad.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IView currentWindow;
        private string statusText;
        private string currentPath;

        private OpenSubtitlesWorker Worker { get; } = new OpenSubtitlesWorker();

        public MainViewModel(IView window)
        {
            currentWindow = window;
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
                statusText = value;
                RaisePropertyChangedEvent("StatusText");
            }
        }

        public ICommand ChooseFileCommand { get => new DelegateCommand(ChooseFile); }
        public ICommand RefreshCommand { get => new DelegateCommand(Refresh); }
        public ICommand SettingsCommand { get => new DelegateCommand(GoToSettings); }
        public ICommand DownloadCommand { get => new DelegateCommand(Download); }

        public void ChooseFile()
        {
            System.Windows.Forms.OpenFileDialog fileChooseDialog = new System.Windows.Forms.OpenFileDialog
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
            SettingsViewModel settingsControl = new SettingsViewModel(this.currentWindow, ApplicationSettings.GetInstance().WantedLanguages);
            this.currentWindow.ChangeCurrentControlTo(settingsControl);
        }

        public async void Refresh()
        {
            if (this.CurrentPath != null)
            {
                await Task.Run(() => this.ProcessFileAsync());
            }
        }

        public async void Download()
        {
            try
            {
                this.StatusText = "Downloading...";
                bool success = await Worker.Download(SelectedItem, Path.ChangeExtension(this.CurrentPath, SelectedItem.GetFormat()));
                if (success)
                {
                    this.StatusText = "Subtitle downloaded.";
                }
                else
                {
                    this.StatusText = "Error while downloading.";
                }
            }
            catch (Exception ex)
            {
                this.StatusText = ex.Message;//"Error while downloading. Try again.";
            }
        }

        private async void ProcessFileAsync()
        {
            try
            {
                StatusText = "Searching subtitles...";
                App.Current.Dispatcher.Invoke(() => this.SubtitleList.Clear());
                var results = await Worker.Search(CurrentPath);
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
    }
}
