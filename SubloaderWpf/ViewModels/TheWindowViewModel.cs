using SubloaderWpf.Interfaces;
using SubloaderWpf.Utilities;

namespace SubloaderWpf.ViewModels;

public class TheWindowViewModel : ViewModelBase, INavigator
{
    private object currentControl;
    private object previousControl = null;

    public TheWindowViewModel()
    {
        CurrentControl = new MainViewModel(this);
        SettingsParser.Saved += () => RaisePropertyChanged(nameof(AlwaysOnTop));
    }

    public bool AlwaysOnTop => App.Settings.KeepWindowOnTop;

    public object CurrentControl
    {
        get => currentControl;

        private set
        {
            previousControl = currentControl;
            Set(nameof(CurrentControl), ref currentControl, value);
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
