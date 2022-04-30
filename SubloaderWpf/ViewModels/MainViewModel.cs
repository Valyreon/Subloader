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
using SubloaderWpf.Interfaces;
using SubloaderWpf.Utilities;
using SuppliersLibrary;
using SuppliersLibrary.Exceptions;
using SuppliersLibrary.OpenSubtitles;

namespace SubloaderWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly List<ISubtitleSupplier> suppliers = new();
        private readonly INavigator navigator;
        private string statusText;
        private string currentPath;
        private bool searchByName;
        private bool searchByHash;

        public MainViewModel(INavigator navigator)
        {
            this.navigator = navigator;

            // Must first add suppliers before processing.
            suppliers.Add(new OpenSubtitles());

            searchByHash = App.Settings.IsByHashChecked;
            searchByName = App.Settings.IsByNameChecked;

            StatusText = "Open a video file.";
            CurrentPath = (Application.Current as App).PathArg;

            App.InstanceMediator.ReceivedArgument += arg => CurrentPath = arg;
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

            set => Set(nameof(StatusText), ref statusText, value);
        }

        public bool SearchByName
        {
            get => searchByName;

            set
            {
                Set(nameof(SearchByName), ref searchByName, value);
                App.Settings.IsByNameChecked = value;
                SettingsParser.Save(App.Settings);
            }
        }

        public bool SearchByHash
        {
            get => searchByHash;

            set
            {
                Set(nameof(SearchByHash), ref searchByHash, value);
                App.Settings.IsByHashChecked = value;
                SettingsParser.Save(App.Settings);
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
                Filter = "Video files |*.wmv; *.3g2; *.3gp; *.3gp2; *.3gpp; *.amv; *.asf;  *.avi; *.bin; " +
                          "*.cue; *.divx; *.dv; *.flv; *.gxf; *.iso; *.m1v; *.m2v; *.m2t; *.m2ts; *.m4v; " +
                          " *.mkv; *.mov; *.mp2; *.mp2v; *.mp4; *.mp4v; *.mpa; *.mpe; *.mpeg; *.mpeg1; " +
                          "*.mpeg2; *.mpeg4; *.mpg; *.mpv2; *.mts; *.nsv; *.nuv; *.ogg; *.ogm; *.ogv; " +
                          "*.ogx; *.ps; *.rec; *.rm; *.rmvb; *.tod; *.ts; *.tts; *.vob; *.vro; *.webm; *.dat; ",
                CheckFileExists = true,
                CheckPathExists = true,
            };

            fileChooseDialog.ShowDialog();

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
                await Task.Run(() =>
                {
                    Thread.Sleep(20);
                    SelectedItem.Model.Download(GetDestinationPath());
                });

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
                // try to focus window
                Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());

                StatusText = "Searching subtitles...";
                Application.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
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
                        Application.Current.Dispatcher.Invoke(() => SubtitleList.Add(x));
                        await Task.Delay(20);
                    }

                    StatusText = "Use button or doubleclick to download.";
                }
            }
            catch (ServerFailException ex)
            {
                StatusText = ex.Message;
                SystemSounds.Hand.Play();
            }
            catch (BadFileException ex)
            {
                StatusText = ex.Message;
                SystemSounds.Hand.Play();
            }
        }

        private async Task<IList<SubtitleEntry>> SearchSuppliers()
        {
            var result = new List<SubtitleEntry>();
            foreach (var supplier in suppliers)
            {
                var results = await supplier.SearchAsync(currentPath, new object[] { SearchByHash, SearchByName });
                foreach (var item in results)
                {
                    var settings = App.Settings;
                    if (settings.WantedLanguages == null ||
                        !settings.WantedLanguages.Any() ||
                        settings.WantedLanguages.Where((subLang) => subLang.Name == item.Language).Any())
                    {
                        result.Add(new SubtitleEntry(item));
                    }
                }
            }

            return result;
        }

        private string GetDestinationPath()
        {
            var directoryPath = App.Settings.DownloadToSubsFolder
                ? Path.Combine(Path.GetDirectoryName(CurrentPath), "Subs")
                : Path.GetDirectoryName(CurrentPath);

            Directory.CreateDirectory(directoryPath);

            if (App.Settings.AllowMultipleDownloads)
            {
                var fileNameWithoutPathOrExtension = Path.GetFileNameWithoutExtension(CurrentPath);
                var path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.{SelectedItem.Model.LanguageID}.{SelectedItem.Model.Format}");

                if (!App.Settings.OverwriteSameLanguageSub && File.Exists(path))
                {
                    var counter = 1;
                    while (File.Exists(
                        path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.({counter}).{SelectedItem.Model.LanguageID}.{SelectedItem.Model.Format}")))
                    {
                        counter++;
                    }
                }

                return path;
            }

            return Path.ChangeExtension(CurrentPath, SelectedItem.Model.Format);
        }
    }
}
