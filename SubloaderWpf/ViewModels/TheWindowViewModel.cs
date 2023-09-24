using System.Windows;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Mvvm;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class TheWindowViewModel : ObservableEntity, INavigator
{
    private readonly IOpenSubtitlesService _openSubtitlesService;
    private readonly ApplicationSettings _settings;
    private bool alwaysOnTop;
    private object currentControl;
    private object previousControl = null;

    public TheWindowViewModel(ApplicationSettings settings, IOpenSubtitlesService openSubtitlesService)
    {
        CurrentControl = new MainViewModel(this, openSubtitlesService, settings);
        _settings = settings;
        _openSubtitlesService = openSubtitlesService;
        AlwaysOnTop = _settings.KeepWindowOnTop;
        ApplicationDataReader.Saved += () => Application.Current.Dispatcher.Invoke(() => AlwaysOnTop = _settings.KeepWindowOnTop);
        _openSubtitlesService = openSubtitlesService;
    }

    public bool AlwaysOnTop { get => alwaysOnTop; set => Set(() => AlwaysOnTop, ref alwaysOnTop, value); }

    public object CurrentControl
    {
        get => currentControl;

        private set
        {
            previousControl = currentControl;
            Set(() => CurrentControl, ref currentControl, value);
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
