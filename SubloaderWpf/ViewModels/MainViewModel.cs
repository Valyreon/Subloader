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

public enum SortBy
{
    Default,
    Language,
    Release
}

public class MainViewModel : ObservableEntity
{
    private readonly INavigator _navigator;
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly Lazy<ApplicationSettings> _settings;
    private string currentPath;
    private bool isSearchModalOpen;
    private string lastSearchedText;
    private string statusText;
    private bool isLoading;
    private ObservableCollection<SubtitleEntry> subtitleList;
    private int currentPage;
    private int totalPages;

    private bool isAscending;
    private SortBy currentSort = SortBy.Default;
    private SearchFormViewModel searchForm;

    public MainViewModel(INavigator navigator, IOpenSubtitlesService openSubtitlesService, Lazy<ApplicationSettings> settings)
    {
        _navigator = navigator;
        _openSubtitlesService = openSubtitlesService;
        _settings = settings;

        StatusText = "Open a video file.";
        CurrentPath = (Application.Current as App).PathArg;

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
                CurrentPage = 1;
                TotalPages = 1;
                currentSort = SortBy.Default;
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

    public int CurrentPage
    {
        get => currentPage;
        set => Set(() => CurrentPage, ref currentPage, value);
    }

    public int TotalPages
    {
        get => totalPages;
        set => Set(() => TotalPages, ref totalPages, value);
    }

    public ICommand OpenSearchModalCommand => new RelayCommand(() =>
    {
        SearchForm ??= new SearchFormViewModel(Search);
        IsSearchModalOpen = true;
    });

    public ICommand RefreshCommand => new RelayCommand(Refresh);

    public SearchFormViewModel SearchForm { get => searchForm; set => Set(() => SearchForm, ref searchForm, value); }

    public ICommand SearchCommand => new RelayCommand(Search);

    private void Search()
    {
        CurrentPage = 1;
        TotalPages = 1;
        currentSort = SortBy.Default;
        SearchPage(CurrentPage);
    }

    public ICommand NextPageCommand => new RelayCommand(NextPage);

    private void NextPage()
    {
        ++CurrentPage;
        Refresh();
    }

    public ICommand PreviousPageCommand => new RelayCommand(PreviousPage);

    private void PreviousPage()
    {
        --CurrentPage;
        Refresh();
    }

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

    public void SortByLanguage()
    {
        if (currentSort == SortBy.Language)
        {
            isAscending = !isAscending;
        }
        else
        {
            isAscending = true;
            currentSort = SortBy.Language;
        }

        ProcessResults(SubtitleList, CurrentPage, TotalPages);
    }

    public void SortByRelease()
    {
        if (currentSort == SortBy.Release)
        {
            isAscending = !isAscending;
        }
        else
        {
            isAscending = true;
            currentSort = SortBy.Release;
        }

        ProcessResults(SubtitleList, CurrentPage, TotalPages);
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
                Filter = $"All files (*.*) |*.*|Subtitle files|*.{_settings.Value.PreferredFormat}",
                FileName = Path.ChangeExtension(SelectedItem.Release, _settings.Value.PreferredFormat)
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
        var settingsControl = new SettingsViewModel(_navigator, _openSubtitlesService, _settings.Value);
        _navigator.GoToControl(settingsControl);
    }

    public void Refresh()
    {
        currentSort = SortBy.Default;
        if (CurrentPath != null)
        {
            ProcessFileAsync();
        }
        else if (!string.IsNullOrWhiteSpace(SearchForm.Text))
        {
            SearchPage(CurrentPage);
        }
    }

    public async void SearchPage(int currentPage)
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
            var result = await _openSubtitlesService.SearchSubtitlesAsync(
                SearchForm.Text,
                SearchForm.Episode,
                SearchForm.Season,
                SearchForm.Year,
                SearchForm.Type,
                SearchForm.ImdbId,
                SearchForm.ParentImdbId,
                currentPage);
            ProcessResults(result.Items, result.CurrentPage, result.TotalPages);
        });
    }

    private async void ProcessFileAsync()
    {
        if (IsLoading)
        {
            return;
        }
        IsLoading = true;

        Application.Current.Dispatcher.Invoke(() => Application.Current.MainWindow.Activate());
        StatusText = "Searching subtitles...";
        SubtitleList = null;
        await RunAndHandleAsync(async () =>
        {
            var result = await _openSubtitlesService.GetSubtitlesForFileAsync(CurrentPath, CurrentPage);
            ProcessResults(result.Items, result.CurrentPage, result.TotalPages);
        });
    }

    private void ProcessResults(IEnumerable<SubtitleEntry> results, int currentPage, int totalPages)
    {
        IsLoading = false;

        CurrentPage = currentPage;
        TotalPages = totalPages;

        if (results == null)
        {
            StatusText = "Server error. Try refreshing.";
            return;
        }
        else if (!results.Any())
        {
            StatusText = "No subtitles found.";
            return;
        }

        SubtitleList = currentSort switch
        {
            SortBy.Default => new ObservableCollection<SubtitleEntry>(results),
            SortBy.Language => new ObservableCollection<SubtitleEntry>(isAscending
                ? results.OrderBy(s => s.Language).ThenBy(s => s.LevenshteinDistance)
                : results.OrderByDescending(s => s.Language).ThenBy(s => s.LevenshteinDistance)),
            SortBy.Release => new ObservableCollection<SubtitleEntry>(isAscending
                ? results.OrderBy(s => s.Release)
                : results.OrderByDescending(s => s.Release)),
            _ => throw new NotImplementedException(),
        };

        StatusText = "Use double-click to download.";

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
                StatusText = ex.Message?.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[0] ?? "Something went wrong.";
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
