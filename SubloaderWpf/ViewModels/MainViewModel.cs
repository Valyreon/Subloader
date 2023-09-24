using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OpenSubtitlesSharp;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Mvvm;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class MainViewModel : ObservableEntity
{
    private readonly INavigator _navigator;
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
    private IEnumerable<SubtitleLanguage> allLanguages;
    private string currentPath;
    private IEnumerable<string> formats;
    private bool isSearchModalOpen;
    private string lastSearchedText;
    private bool searchByHash;
    private bool searchByName;
    private string searchModalInputText;
    private string statusText;

    public MainViewModel(INavigator navigator, IOpenSubtitlesService openSubtitlesService, ApplicationSettings settings)
    {
        _navigator = navigator;
        _openSubtitlesService = openSubtitlesService;
        _settings = settings;
        searchByHash = _settings.IsByHashChecked;
        searchByName = _settings.IsByNameChecked;

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
        set => Set(() => IsSearchModalOpen, ref isSearchModalOpen, value);
    }

    public ICommand OpenSearchModalCommand => new RelayCommand(() => IsSearchModalOpen = true);
    public ICommand RefreshCommand => new RelayCommand(Refresh);

    public bool SearchByHash
    {
        get => searchByHash;

        set
        {
            Set(() => SearchByHash, ref searchByHash, value);
            _settings.IsByHashChecked = value;
            _ = SettingsParser.SaveAsync(_settings);
        }
    }

    public bool SearchByName
    {
        get => searchByName;

        set
        {
            Set(() => SearchByName, ref searchByName, value);
            _settings.IsByNameChecked = value;
            _ = SettingsParser.SaveAsync(_settings);
        }
    }

    public ICommand SearchCommand => new RelayCommand(Search);

    public string SearchModalInputText
    {
        get => searchModalInputText;
        set => Set(() => SearchModalInputText, ref searchModalInputText, value);
    }

    public SubtitleEntry SelectedItem { get; set; }
    public ICommand SettingsCommand => new RelayCommand(GoToSettings);

    public string StatusText
    {
        get => statusText;

        set => Set(() => StatusText, ref statusText, value);
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

        string destination = null;
        if (string.IsNullOrWhiteSpace(CurrentPath))
        {
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = Path.ChangeExtension(SelectedItem.Name, _settings.PreferredFormat)
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

        StatusText = "Downloading...";
        await RunAndHandleAsync(async () =>
        {
            var info = await _openSubtitlesService.DownloadSubtitleAsync(SelectedItem, CurrentPath, destination);
            Application.Current.Dispatcher.Invoke(() => StatusText = $"Subtitle downloaded. Remaining today: " + info.Remaining);
        });
    }

    public async void GoToSettings()
    {
        if (allLanguages == null || formats == null)
        {
            var languageTask = allLanguages == null ? _openSubtitlesService.GetLanguagesAsync() : Task.FromResult(allLanguages);
            var formatsTask = formats == null ? _openSubtitlesService.GetFormatsAsync() : Task.FromResult(formats);

            await Task.WhenAll(languageTask, formatsTask);

            allLanguages ??= languageTask.Result;
            formats ??= formatsTask.Result;
        }

        var settingsControl = new SettingsViewModel(_navigator, _openSubtitlesService, _settings, allLanguages, formats);
        _navigator.GoToControl(settingsControl);
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

        Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());
        StatusText = "Searching subtitles...";
        Application.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
        await RunAndHandleAsync(async () =>
        {
            var results = await _openSubtitlesService.SearchSubtitlesAsync(SearchModalInputText);
            await ProcessResults(results);
        });
    }

    private async void ProcessFileAsync()
    {
        Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());
        StatusText = "Searching subtitles...";
        Application.Current.Dispatcher.Invoke(() => SubtitleList.Clear());
        await RunAndHandleAsync(async () =>
        {
            var results = await _openSubtitlesService.GetSubtitlesForFileAsync(CurrentPath, SearchByName, SearchByHash);
            await ProcessResults(results);
        });
    }

    private async Task ProcessResults(IEnumerable<SubtitleEntry> results)
    {
        if (results == null)
        {
            Application.Current.Dispatcher.Invoke(() => StatusText = "Server error. Try refreshing.");
        }
        else if (!results.Any())
        {
            Application.Current.Dispatcher.Invoke(() => StatusText = "No subtitles found.");
        }
        else
        {
            foreach (var x in results)
            {
                Application.Current.Dispatcher.Invoke(() => SubtitleList.Add(x));
                await Task.Delay(20);
            }

            Application.Current.Dispatcher.Invoke(() => StatusText = "Use double-click to download.");
        }
    }

    private Task RunAndHandleAsync(Func<Task> func)
    {
        return Task.Run(async () =>
        {
            try
            {
                await func();
            }
            catch (RequestFailedException ex)
            {
                Application.Current.Dispatcher.Invoke(() => StatusText = ex.Message);
                SystemSounds.Hand.Play();
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() => StatusText = "Something went wrong.");
                SystemSounds.Hand.Play();
            }
        });
    }
}
