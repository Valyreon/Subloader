using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Fastenshtein;
using Microsoft.Win32;
using OpenSubtitlesSharp;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class MainViewModel : ViewModelBase
{
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
            try
            {
                using var osClient = new OpenSubtitlesClient(App.APIKey, App.Settings.LoginToken, App.Settings.BaseUrl);
                var downloadInfo = await osClient.GetDownloadInfoAsync(SelectedItem.Model.Information.Files.First().FileId.Value);
                var extension = Path.GetExtension(downloadInfo.FileName);

                string destination;
                if (string.IsNullOrWhiteSpace(CurrentPath))
                {
                    var saveFileDialog = new SaveFileDialog()
                    {
                        FileName = Path.ChangeExtension(SelectedItem.Name, extension)
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
                    destination = GetDestinationPath(extension);
                }

                File.WriteAllBytes(destination, await DownloadFileAsync(downloadInfo.Link));
                Application.Current.Dispatcher.Invoke(() => StatusText = $"Subtitle downloaded. Remaining: " + downloadInfo.Remaining);
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => StatusText = "Error while downloading.");
                SystemSounds.Hand.Play();
            }
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

    private static async Task<byte[]> DownloadFileAsync(string url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url);

            return response.IsSuccessStatusCode
                ? await response.Content.ReadAsByteArrayAsync()
                : null;
        }
        catch (HttpRequestException e)
        {
            return null;
        }
    }

    private string GetDestinationPath(string format)
    {
        format = format.StartsWith(".") ? format[1..] : format;

        var directoryPath = App.Settings.DownloadToSubsFolder
            ? Path.Combine(Path.GetDirectoryName(CurrentPath), "Subs")
            : Path.GetDirectoryName(CurrentPath);

        Directory.CreateDirectory(directoryPath);

        if (App.Settings.AllowMultipleDownloads)
        {
            var fileNameWithoutPathOrExtension = Path.GetFileNameWithoutExtension(CurrentPath);
            var path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.{SelectedItem.Model.Information.Language}.{format}");

            if (!App.Settings.OverwriteSameLanguageSub && File.Exists(path))
            {
                var counter = 1;
                while (File.Exists(
                    path = Path.Combine(directoryPath, $"{fileNameWithoutPathOrExtension}.({counter}).{SelectedItem.Model.Information.Language}.{format}")))
                {
                    counter++;
                }
            }

            return path;
        }

        return Path.ChangeExtension(CurrentPath, format);
    }

    private async Task GetResults(bool fileSearch)
    {
        try
        {
            // try to focus window
            Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());

            StatusText = "Searching subtitles...";
            Application.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
            var results = await SearchOpensubtitles(fileSearch);
            if (results == null)
            {
                StatusText = "Server error. Try refreshing.";
            }
            else if (!results.Any())
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

    private async Task<IEnumerable<SubtitleEntry>> SearchOpensubtitles(bool forFile = true)
    {
        var settings = App.Settings;
        using var newClient = new OpenSubtitlesClient(App.APIKey);

        var parameters = new SearchParameters
        {
            Languages = App.Settings.WantedLanguages,
            OnlyMovieHashMatch = SearchByHash && !SearchByName
        };

        SearchResult result = null;

        if (forFile)
        {
            result = await newClient.SearchAsync(currentPath, parameters);
            // order by levenshtein distance
            var laven = new Levenshtein(Path.GetFileNameWithoutExtension(CurrentPath));
            return result.Items.Select(i => new SubtitleEntry(i))
                .Select(ResultItem => (ResultItem, laven.DistanceFrom(ResultItem.Name)))
                .OrderBy(i => i.Item2)
                .Select(i => i.ResultItem);
        }
        parameters.Query = SearchModalInputText;
        result = await newClient.SearchAsync(parameters);

        return result.Items.Select(i => new SubtitleEntry(i));
    }
}
