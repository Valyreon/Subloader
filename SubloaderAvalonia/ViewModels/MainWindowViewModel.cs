using ReactiveUI;
using SubloaderAvalonia.Interfaces;
using SubloaderAvalonia.Models;
using SubloaderAvalonia.Utilities;

namespace SubloaderAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase, INavigator
{
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
    private bool alwaysOnTop;
    private object currentControl;
    private object previousControl = null;

    public MainWindowViewModel(ApplicationSettings settings, IOpenSubtitlesService openSubtitlesService)
    {
        CurrentControl = new MainViewModel(this, openSubtitlesService, settings);
        _settings = settings;
        _openSubtitlesService = openSubtitlesService;
        AlwaysOnTop = _settings.KeepWindowOnTop;
        ApplicationDataReader.Saved += () => AlwaysOnTop = _settings.KeepWindowOnTop;
        _openSubtitlesService = openSubtitlesService;
    }

    public bool AlwaysOnTop { get => alwaysOnTop; set => this.RaiseAndSetIfChanged(ref alwaysOnTop, value); }

    public object CurrentControl
    {
        get => currentControl;

        private set
        {
            previousControl = currentControl;
            currentControl = value;
            this.RaisePropertyChanged(nameof(CurrentControl));
        }
    }

    public void GoToControl(object control)
    {
        CurrentControl = control;
    }

    public void GoToPreviousControl()
    {
        CurrentControl = previousControl;
        previousControl = null;
    }
}
