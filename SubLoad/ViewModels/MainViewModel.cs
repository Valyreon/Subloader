using Microsoft.Win32;
using SubLib;
using SubLoad.Models;
using SubLoad.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace SubLoad.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IView currentWindow;
        private string statusText;
        private string currentPath = (Application.Current as App).PathArg;

        private List<SubtitleLanguage> wantedLanguages;
        private bool shouldReadConfig = true;

        public MainViewModel(IView window)
        {
            currentWindow = window;
            StatusText = "Open a video file.";
        }

        public ObservableCollection<SubtitleEntry> SubtitleList { get; set; } = new ObservableCollection<SubtitleEntry>();

        public SubtitleEntry SelectedItem { get; set; }

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
        public ICommand MainLoadedCommand { get => new DelegateCommand(LoadConfig); }

        public void LoadConfig()
        {
            if (shouldReadConfig)
            {
                wantedLanguages = ApplicationSettings.LoadApplicationSettings();
            }
        }

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
                this.ProcessFileAsync(this.currentPath = fileInfo.FullName);
            }
            catch (Exception)
            {
                this.StatusText = "Open a video file.";
            }
        }

        public void GoToSettings()
        {
            SettingsViewModel settingsControl = new SettingsViewModel(this.currentWindow, wantedLanguages);
            this.currentWindow.ChangeCurrentControlTo(settingsControl);
            shouldReadConfig = true;
        }

        public async void Refresh()
        {
            if (this.currentPath != null)
            {
                await Task.Run(() => this.ProcessFileAsync(this.currentPath)); //await this
            }
        }

        public async void Download()
        {
            using (OSIntermediary messenger = new OSIntermediary())
            {
                await messenger.OSLogIn();
                try
                {
                    if (SelectedItem != null)
                    {
                        this.StatusText = "Downloading...";
                        byte[] subtitleStream = await messenger.DownloadSubtitle(SelectedItem.GetSubtitleFileID());

                        if (subtitleStream != null)
                        {
                            File.WriteAllBytes(Path.ChangeExtension(this.currentPath, SelectedItem.GetFormat()), subtitleStream);
                            this.StatusText = "Subtitle downloaded.";
                        }
                        else
                        {
                            this.StatusText = "Error while downloading.";
                        }
                    }
                }
                catch (Exception)
                {
                    this.StatusText = "Error while downloading. Try again.";
                }
            }
        }

        private async void ProcessFileAsync(string path)
        {
            LoadConfig();

            SearchSubtitlesResponse ssre = null;
            using (OSIntermediary messenger = new OSIntermediary())
            {
                await messenger.OSLogIn();
                Application.Current.Dispatcher.Invoke(() => this.SubtitleList.Clear());
                this.StatusText = "Searching subtitles...";
                ssre = await messenger.SearchOS(currentPath, "all");
            }

            if (ssre != null && (ssre.data == null || ssre.data.Length == 0))
            {
                this.StatusText = "No subtitles found.";
            }
            else if (ssre == null)
            {
                this.StatusText = "Server error. Try refreshing.";
            }
            else
            {
                foreach (var x in ssre.data)
                {
                    if (wantedLanguages == null || wantedLanguages.Count==0 || wantedLanguages.Where((subLang) => subLang.Name == x.LanguageName).Any())
                    {
                        App.Current.Dispatcher.Invoke(
                            () => { this.SubtitleList.Add(new SubtitleEntry(x.SubFileName, x.LanguageName, int.Parse(x.IDSubtitleFile), x.SubFormat)); });
                        await Task.Run(() => Thread.Sleep(20));
                    }
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
    }
}
