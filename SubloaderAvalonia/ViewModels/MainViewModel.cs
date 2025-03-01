using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using OpenSubtitlesSharp;
using ReactiveUI;
using SubloaderAvalonia.Interfaces;
using SubloaderAvalonia.Models;

namespace SubloaderAvalonia.ViewModels;

public enum SortBy
{
    Default,
    Language,
    Release
}

public class MainViewModel : ViewModelBase
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
    private int currentPage;
    private int totalPages;

    private bool isAscending;
    private SortBy currentSort = SortBy.Default;

    // for the designer
    public MainViewModel()
    {

    }

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

    public ReactiveCommand<Unit, Unit> ChooseFileCommand => ReactiveCommand.Create(ChooseFile);
    public ReactiveCommand<Unit, Unit> CloseSearchModalCommand => ReactiveCommand.Create(() => { IsSearchModalOpen = false; SearchForm.Text = lastSearchedText; });

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

    public ReactiveCommand<Unit, Unit> DownloadCommand => ReactiveCommand.Create(Download);

    public bool IsSearchModalOpen
    {
        get => isSearchModalOpen;
        set => this.RaiseAndSetIfChanged(ref isSearchModalOpen, value);
    }

    public bool IsLoading
    {
        get => isLoading;
        set => this.RaiseAndSetIfChanged(ref isLoading, value);
    }

    public int CurrentPage
    {
        get => currentPage;
        set => this.RaiseAndSetIfChanged(ref currentPage, value);
    }

    public int TotalPages
    {
        get => totalPages;
        set => this.RaiseAndSetIfChanged(ref totalPages, value);
    }

    public ReactiveCommand<Unit, bool> OpenSearchModalCommand => ReactiveCommand.Create(() => IsSearchModalOpen = true);

    public ReactiveCommand<Unit, Unit> RefreshCommand => ReactiveCommand.Create(Refresh);

    public SearchFormViewModel SearchForm { get; set; }

    public ReactiveCommand<Unit, Unit> SearchCommand => ReactiveCommand.Create(Search);

    private void Search()
    {
        CurrentPage = 1;
        TotalPages = 1;
        currentSort = SortBy.Default;
        SearchPage(CurrentPage);
    }

    public ReactiveCommand<Unit, Unit> NextPageCommand => ReactiveCommand.Create(NextPage);

    private void NextPage()
    {
        ++CurrentPage;
        Refresh();
    }

    public ReactiveCommand<Unit, Unit> PreviousPageCommand => ReactiveCommand.Create(PreviousPage);

    private void PreviousPage()
    {
        --CurrentPage;
        Refresh();
    }

    public SubtitleEntry SelectedItem { get; set; }
    public ReactiveCommand<Unit, Unit> SettingsCommand => ReactiveCommand.Create(GoToSettings);

    public string StatusText
    {
        get => statusText;
        set => this.RaiseAndSetIfChanged(ref statusText, value);
    }

    public ObservableCollection<SubtitleEntry> SubtitleList
    {
        get => subtitleList;
        set => this.RaiseAndSetIfChanged(ref subtitleList, value);
    }

    public async void ChooseFile()
    {
        if(Application.Current.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Not a desktop.");
            return;
        }

        if(!desktop.MainWindow.StorageProvider.CanOpen)
        {
            Console.WriteLine("Can't open file picker on current platform.");
            return;
        }

        try
        {
            var topLevel = TopLevel.GetTopLevel(desktop.MainWindow);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                FileTypeFilter = [
                new FilePickerFileType("Video files")
                {
                    Patterns ="*.mp4; *.mkv; *.avi; *.wmv; *.mov; *.flv; *.webm; *.3gp; *.mpeg; *.ogv; *.rmvb; *.vob; *.mts; *.m2ts; *.wav; *.mpg;".Split(';', StringSplitOptions.TrimEntries)
                },
                new FilePickerFileType("All files")
                {
                    Patterns ="*.*;".Split(';', StringSplitOptions.TrimEntries)
                },
            ],
                AllowMultiple = false,
                Title = "Choose video file"
            });

            if(files.Count == 0)
            {
                return;
            }

            SubtitleList = null;
            CurrentPath = files[0].Path.GetComponents(UriComponents.Path, UriFormat.Unescaped);
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
            /*var saveFileDialog = new SaveFileDialog()
            {
                Filter = $"All files (*.*) |*.*|Subtitle files|*.{_settings.PreferredFormat}",
                FileName = Path.ChangeExtension(SelectedItem.Release, _settings.PreferredFormat)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                destination = saveFileDialog.FileName;
            }
            else
            {
                return;
            }*/
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

        (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow.Activate();
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

        (Application.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime).MainWindow.Activate();
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
            }
            catch (Exception)
            {
                StatusText = "Something went wrong.";
            }
            finally
            {
                IsLoading = false;
            }
        });
    }
}
