namespace SubloaderWpf.ViewModels
{
    public class TheWindowViewModel : ViewModelBase, INavigator
    {
        private object previousControl = null;

        private object currentControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralViewModel"/>.
        /// </summary>
        /// <param name="thisWindow">Window in which all the UserControls are to be shown in.</param>
        public TheWindowViewModel() => CurrentControl = new MainViewModel(this);

        public object CurrentControl
        {
            get => currentControl;

            internal set
            {
                previousControl = currentControl;
                Set("CurrentControl", ref currentControl, value);
            }
        }

        public void GoToControl(object control) => CurrentControl = control;

        public void GoToPreviousControl()
        {
            CurrentControl = previousControl;
            previousControl = null;
        }
    }
}
