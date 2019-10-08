using CommonServiceLocator;
using GalaSoft.MvvmLight;

namespace SubLoad.ViewModels
{
    public class GeneralWindowViewModel : ViewModelBase, INavigator
    {
        private object previousControl = null;

        private object currentControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralViewModel"/>.
        /// </summary>
        /// <param name="thisWindow">Window in which all the UserControls are to be shown in.</param>
        public GeneralWindowViewModel()
        {
            this.CurrentControl = new MainViewModel(this);
        }

        public object CurrentControl
        {
            get
            {
                return this.currentControl;
            }

            internal set
            {
                previousControl = this.currentControl;
                Set("CurrentControl", ref currentControl, value);
            }
        }

        public void GoToControl(object control)
        {
            this.CurrentControl = control;
        }

        public void GoToPreviousControl()
        {
            this.CurrentControl = previousControl;
            previousControl = null;
        }
    }
}
