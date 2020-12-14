using SubloaderWpf.Interfaces;
using SubloaderWpf.Models;

namespace SubloaderWpf.ViewModels
{
    public class TheWindowViewModel : ViewModelBase, INavigator
    {
        private object previousControl = null;

        private object currentControl;
        private bool alwaysOnTop;

        public TheWindowViewModel()
        {
            CurrentControl = new MainViewModel(this);
            AlwaysOnTop = ApplicationSettings.Instance.KeepWindowOnTop;

            ApplicationSettings.Changed += () => AlwaysOnTop = ApplicationSettings.Instance.KeepWindowOnTop;
        }

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

        public bool AlwaysOnTop
        {
            get => alwaysOnTop;
            set => Set("AlwaysOnTop", ref alwaysOnTop, value);
        }
    }
}
