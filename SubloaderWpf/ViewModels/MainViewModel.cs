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

namespace SubloaderWpf.ViewModels;

public class MainViewModel : ObservableEntity
{
    private readonly INavigator _navigator;
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
    private string currentPath;
    private bool isSearchModalOpen;
    private string lastSearchedText;
    private string statusText;
    private bool isLoading;
    private ObservableCollection<SubtitleEntry> subtitleList;

    public MainViewModel(INavigator navigator, IOpenSubtitlesService openSubtitlesService, ApplicationSettings settings)
    {
        _navigator = navigator;
        _openSubtitlesService = openSubtitlesService;
        _settings = settings;

        StatusText = "Open a video file.";
        CurrentPath = (Application.Current as App).PathArg;
        SearchForm = new SearchFormViewModel(Search);

        App.InstanceMediator.ReceivedArgument += arg => CurrentPath = arg;
    }

    public ICommand ChooseFileCommand => new RelayCommand(ChooseFile);
    public ICommand CloseSearchModalCommand => new RelayCommand(() => { IsSearchModalOpen = false; SearchForm.Text = lastSearchedText; });

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

    public bool IsLoading
    {
        get => isLoading;
        set => Set(() => IsLoading, ref isLoading, value);
    }

    public ICommand OpenSearchModalCommand => new RelayCommand(() => IsSearchModalOpen = true);

    public ICommand RefreshCommand => new RelayCommand(Refresh);

    public SearchFormViewModel SearchForm { get; set; }


    public ICommand SearchCommand => new RelayCommand(Search);

    public SubtitleEntry SelectedItem { get; set; }
    public ICommand SettingsCommand => new RelayCommand(GoToSettings);

    public string StatusText
    {
        get => statusText;
        set => Set(() => StatusText, ref statusText, value);
    }

    public ObservableCollection<SubtitleEntry> SubtitleList
    {
        get => subtitleList;
        set => Set(() => SubtitleList, ref subtitleList, value);
    }

    public void ChooseFile()
    {
        var fileChooseDialog = new OpenFileDialog
        {
            Filter = "Video files |*.mp4; *.mkv; *.avi; *.wmv; *.mov; *.flv; *.webm; *.3gp; *.mpeg; *.ogv; *.rmvb; *.vob; *.mts; *.m2ts; *.wav; *.mpg;| AllFiles |*.*;",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        fileChooseDialog.ShowDialog();

        try
        {
            var fileInfo = new FileInfo(fileChooseDialog.FileName);
            SubtitleList = null;
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
                Filter = $"All files (*.*) |*.*|Subtitle files|*.{_settings.PreferredFormat}",
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
            StatusText = $"Subtitle downloaded. Remaining today: " + info.Remaining;
        });
    }

    public void GoToSettings()
    {
        var settingsControl = new SettingsViewModel(_navigator, _openSubtitlesService, _settings);
        _navigator.GoToControl(settingsControl);
    }

    public void Refresh()
    {
        if (CurrentPath != null)
        {
            ProcessFileAsync();
        }
        else if (!string.IsNullOrWhiteSpace(SearchForm.Text))
        {
            Search();
        }
    }

    public async void Search()
    {
        if (IsLoading)
        {
            return;
        }
        IsLoading = true;
        SubtitleList = null;

        IsSearchModalOpen = false;
        CurrentPath = null;
        if (string.IsNullOrWhiteSpace(SearchForm.Text) &&
            !SearchForm.Episode.HasValue &&
            !SearchForm.Season.HasValue &&
            !SearchForm.Year.HasValue &&
            !SearchForm.ImdbId.HasValue &&
            !SearchForm.ParentImdbId.HasValue)
        {
            StatusText = "Not enough parameters";
            IsLoading = false;
            return;
        }

        lastSearchedText = SearchForm.Text;

        Application.Current.MainWindow.Activate();
        StatusText = "Searching subtitles...";
        await RunAndHandleAsync(async () =>
        {
            var results = await _openSubtitlesService.SearchSubtitlesAsync(
                SearchForm.Text,
                SearchForm.Episode,
                SearchForm.Season,
                SearchForm.Year,
                SearchForm.Type,
                SearchForm.ImdbId,
                SearchForm.ParentImdbId);
            ProcessResults(results);
        });
    }

    private async void ProcessFileAsync()
    {
        if (IsLoading)
        {
            return;
        }
        IsLoading = true;

        Application.Current.MainWindow.Activate();
        StatusText = "Searching subtitles...";
        SubtitleList = null;
        await RunAndHandleAsync(async () =>
        {
            var results = await _openSubtitlesService.GetSubtitlesForFileAsync(CurrentPath);
            ProcessResults(results);
        });
    }

    private void ProcessResults(IEnumerable<SubtitleEntry> results)
    {
        IsLoading = false;

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
            SubtitleList = new ObservableCollection<SubtitleEntry>(results);
            StatusText = "Use double-click to download.";
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
                StatusText = ex.Message;
                SystemSounds.Hand.Play();
            }
            catch (Exception)
            {
                StatusText = "Something went wrong.";
                SystemSounds.Hand.Play();
            }
            finally
            {
                IsLoading = false;
            }
        });
    }
}
