using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;

namespace SubloaderWpf.ViewModels
{
    public class TheWindowViewModel : ViewModelBase, INavigator
    {
        private object previousControl = null;

        private object currentControl;

        public TheWindowViewModel()
        {
            CurrentControl = new MainViewModel(this);
            SettingsParser.Saved += () => RaisePropertyChanged("AlwaysOnTop");
        }

        public bool AlwaysOnTop => App.Settings.KeepWindowOnTop;

        public object CurrentControl
        {
            get => currentControl;

            private set
            {
                previousControl = currentControl;
                Set("CurrentControl", ref currentControl, value);
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
}
