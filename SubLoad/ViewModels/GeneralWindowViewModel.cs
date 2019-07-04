using SubLoad.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SubLoad.ViewModels
{
    public class GeneralWindowViewModel : ObservableObject
    {
        private readonly IView thisWindow;

        private object currentControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralViewModel"/>.
        /// </summary>
        /// <param name="thisWindow">Window in which all the UserControls are to be shown in.</param>
        public GeneralWindowViewModel(IView thisWindow)
        {
            this.thisWindow = thisWindow;
            MainViewModel mainModel = new MainViewModel(this.thisWindow);
            this.CurrentControl = mainModel;
        }

        public object CurrentControl
        {
            get
            {
                return this.currentControl;
            }

            internal set
            {
                this.currentControl = value;
                this.RaisePropertyChangedEvent("CurrentControl");
            }
        }
    }
}
