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
using SuppliersLibrary.Exceptions;
using SuppliersLibrary.OpenSubtitles;

namespace SubloaderWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly OpenSubtitlesClient client = new();
        private readonly INavigator navigator;
        private string currentPath;
        private bool isSearchModalOpen;
        private string lastSearchedText;
        private bool searchByHash;
        private bool searchByName;
        private string searchModalInputText;
        private string statusText;

        public MainViewModel(INavigator navigator)
        {
            this.navigator = navigator;

            searchByHash = App.Settings.IsByHashChecked;
            searchByName = App.Settings.IsByNameChecked;

            StatusText = "Open a video file.";
            CurrentPath = (Application.Current as App).PathArg;

            App.InstanceMediator.ReceivedArgument += arg => CurrentPath = arg;
        }

        public ICommand ChooseFileCommand => new RelayCommand(ChooseFile);
        public ICommand CloseSearchModalCommand => new RelayCommand(() => { IsSearchModalOpen = false; SearchModalInputText = lastSearchedText; });

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

        public ICommand DownloadCommand => new RelayCommand(Download);

        public bool IsSearchModalOpen
        {
            get => isSearchModalOpen;

            set => Set(nameof(IsSearchModalOpen), ref isSearchModalOpen, value);
        }

        public ICommand OpenSearchModalCommand => new RelayCommand(() => IsSearchModalOpen = true);
        public ICommand RefreshCommand => new RelayCommand(Refresh);

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

        public ICommand SearchCommand => new RelayCommand(Search);

        public string SearchModalInputText
        {
            get => searchModalInputText;

            set => Set(nameof(SearchModalInputText), ref searchModalInputText, value);
        }

        public SubtitleEntry SelectedItem { get; set; }
        public ICommand SettingsCommand => new RelayCommand(GoToSettings);

        public string StatusText
        {
            get => statusText;

            set => Set(nameof(StatusText), ref statusText, value);
        }

        public ObservableCollection<SubtitleEntry> SubtitleList { get; set; } = new ObservableCollection<SubtitleEntry>();

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

        public async void Download()
        {
            if (SelectedItem == null)
            {
                return;
            }

            StatusText = "Downloading...";
            await Task.Run(async () =>
            {
                string destination;
                if (string.IsNullOrWhiteSpace(CurrentPath))
                {
                    var saveFileDialog = new SaveFileDialog()
                    {
                        FileName = SelectedItem.Name
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        destination = saveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    destination = GetDestinationPath();
                }

                try
                {
                    await client.Download(SelectedItem.Model, destination);
                }
                catch (Exception)
                {
                    Application.Current.Dispatcher.Invoke(() => StatusText = "Error while downloading.");
                    SystemSounds.Hand.Play();
                }

                Application.Current.Dispatcher.Invoke(() => StatusText = "Subtitle downloaded.");
            });
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
            else if (!string.IsNullOrWhiteSpace(SearchModalInputText))
            {
                Search();
            }
        }

        public async void Search()
        {
            IsSearchModalOpen = false;
            CurrentPath = null;
            if (string.IsNullOrWhiteSpace(SearchModalInputText))
            {
                return;
            }

            lastSearchedText = SearchModalInputText;
            await GetResults(false);
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

        private async Task GetResults(bool fileSearch)
        {
            try
            {
                // try to focus window
                Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());

                StatusText = "Searching subtitles...";
                Application.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
                var results = await SearchSuppliers(fileSearch);
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

                    StatusText = "Use double-click to download.";
                }
            }
            catch (Exception ex)
            {
                StatusText = ex.Message;
                SystemSounds.Hand.Play();
            }
        }

        private async void ProcessFileAsync()
        {
            await GetResults(true);
        }

        private async Task<IReadOnlyList<SubtitleEntry>> SearchSuppliers(bool forFile = true)
        {
            var settings = App.Settings;
            var result = new List<SubtitleEntry>();

            var langCode = settings.WantedLanguages?.Count == 1
                ? settings.WantedLanguages.Single().Code
                : null;

            var results = forFile ? await client.SearchForFileAsync(currentPath, SearchByHash, SearchByName, langCode)
                                  : await client.SearchAsync(SearchModalInputText, langCode);

            if (!string.IsNullOrWhiteSpace(langCode) || settings.WantedLanguages?.Any() != true)
            {
                return results.Select(i => new SubtitleEntry(i)).ToList();
            }

            return results.Where(item => settings.WantedLanguages.Any(subLang => subLang.Name == item.Language))
                .Select(i => new SubtitleEntry(i))
                .ToList();
        }
    }
}
