using System;
using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;
using SubloaderWpf.Mvvm;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class TheWindowViewModel : ObservableEntity, INavigator
{
    private readonly Lazy<ApplicationSettings> _settings;
    private bool alwaysOnTop;
    private object currentControl;
    private object previousControl = null;

    public TheWindowViewModel(Lazy<ApplicationSettings> settings, IOpenSubtitlesService openSubtitlesService)
    {
        CurrentControl = new MainViewModel(this, openSubtitlesService, settings);
        _settings = settings;
        ApplicationDataReader.Saved += () => AlwaysOnTop = _settings.Value.KeepWindowOnTop;
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

    public void Load()
    {
        AlwaysOnTop = _settings.Value.KeepWindowOnTop;
    }
}
