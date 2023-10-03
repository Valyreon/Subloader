using System;
using System.Reactive;
using OpenSubtitlesSharp;
using ReactiveUI;

namespace SubloaderAvalonia.ViewModels;

public class SearchFormViewModel : ViewModelBase
{
    private readonly Action searchAction;
    public int? Episode { get; set; }

    public int? Season { get; set; }

    public int? Year { get; set; }

    public int? ParentImdbId { get; set; }

    public int? ImdbId { get; set; }

    public FileTypeFilter Type { get; set; }

    private string text;

    public SearchFormViewModel(Action searchAction)
    {
        SearchTypeSelectedIndex = 2;
        this.searchAction = searchAction;
    }

    public ReactiveCommand<Unit, Unit> SearchCommand => ReactiveCommand.Create(searchAction);

    public string Text
    {
        get => text;
        set => this.RaiseAndSetIfChanged(ref text, value);
    }

    private bool areTvShowFiltersEnabled;

    public bool AreTvShowFiltersEnabled
    {
        get => areTvShowFiltersEnabled;
        set
        {
            this.RaiseAndSetIfChanged(ref areTvShowFiltersEnabled, value);
            if(!value)
            {
                EpisodeText = string.Empty;
                SeasonText = string.Empty;
                ParentImdbIdText = string.Empty;
            }
        }
    }

    private string episodeText;

    public string EpisodeText
    {
        get => episodeText;
        set
        {
            if (episodeText != value)
            {
                episodeText = value;
                this.RaisePropertyChanged(nameof(EpisodeText));

                Episode = string.IsNullOrWhiteSpace(episodeText)
                    ? null
                    : int.Parse(episodeText);
            }
        }
    }

    private string seasonText;

    public string SeasonText
    {
        get => seasonText;
        set
        {
            if (seasonText != value)
            {
                seasonText = value;
                this.RaisePropertyChanged(nameof(SeasonText));

                Season = string.IsNullOrWhiteSpace(seasonText)
                    ? null
                    : int.Parse(seasonText);
            }
        }
    }

    private string yearText;

    public string YearText
    {
        get => yearText;
        set
        {
            if (yearText != value)
            {
                yearText = value;
                this.RaisePropertyChanged(nameof(YearText));

                Year = string.IsNullOrWhiteSpace(yearText)
                    ? null
                    : int.Parse(yearText);
            }
        }
    }

    private string imdbIdText;

    public string ImdbIdText
    {
        get => imdbIdText;
        set
        {
            if (imdbIdText != value)
            {
                imdbIdText = value;
                this.RaisePropertyChanged(nameof(ImdbIdText));

                ImdbId = string.IsNullOrWhiteSpace(imdbIdText)
                    ? null
                    : int.Parse(imdbIdText);
            }
        }
    }

    private string parentImdbIdText;

    public string ParentImdbIdText
    {
        get => parentImdbIdText;
        set
        {
            if (parentImdbIdText != value)
            {
                parentImdbIdText = value;
                this.RaisePropertyChanged(nameof(ParentImdbIdText));

                ParentImdbId = string.IsNullOrWhiteSpace(parentImdbIdText)
                    ? null
                    : int.Parse(parentImdbIdText);
            }
        }
    }

    private int searchTypeSelectedIndex;

    public int SearchTypeSelectedIndex
    {
        get => searchTypeSelectedIndex;
        set
        {
            if (searchTypeSelectedIndex != value)
            {
                searchTypeSelectedIndex = value;
                this.RaisePropertyChanged(nameof(SearchTypeSelectedIndex));

                Type = (FileTypeFilter)searchTypeSelectedIndex;
                AreTvShowFiltersEnabled = Type is FileTypeFilter.Episode or FileTypeFilter.All;

            }
        }
    }
}
