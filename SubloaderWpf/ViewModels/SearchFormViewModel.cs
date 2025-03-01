using System;
using System.Text.RegularExpressions;
using System.Windows.Input;
using OpenSubtitlesSharp;
using SubloaderWpf.Mvvm;

namespace SubloaderWpf.ViewModels;

public partial class SearchFormViewModel : ObservableEntity
{
    [GeneratedRegex(@"^\d*$")]
    private static partial Regex NumberRegex();

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

    public ICommand SearchCommand => new RelayCommand(searchAction);

    public string Text
    {
        get => text;
        set => Set(() => Text, ref text, value);
    }

    private bool areTvShowFiltersEnabled;

    public bool AreTvShowFiltersEnabled
    {
        get => areTvShowFiltersEnabled;
        set
        {
            Set(() => AreTvShowFiltersEnabled, ref areTvShowFiltersEnabled, value);
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
            if (episodeText != value && NumberRegex().IsMatch(value))
            {
                episodeText = value;
                RaisePropertyChanged(() => EpisodeText);

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
            if (seasonText != value && NumberRegex().IsMatch(value))
            {
                seasonText = value;
                RaisePropertyChanged(() => SeasonText);

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
            if (yearText != value && NumberRegex().IsMatch(value))
            {
                yearText = value;
                RaisePropertyChanged(() => YearText);

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
            if (imdbIdText != value && NumberRegex().IsMatch(value))
            {
                imdbIdText = value;
                RaisePropertyChanged(() => ImdbIdText);

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
            if (parentImdbIdText != value && NumberRegex().IsMatch(value))
            {
                parentImdbIdText = value;
                RaisePropertyChanged(() => ParentImdbIdText);

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
                RaisePropertyChanged(() => SearchTypeSelectedIndex);

                Type = (FileTypeFilter)searchTypeSelectedIndex;
                AreTvShowFiltersEnabled = Type is FileTypeFilter.Episode or FileTypeFilter.All;

            }
        }
    }
}
