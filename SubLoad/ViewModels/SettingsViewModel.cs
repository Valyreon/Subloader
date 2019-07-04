using SubLoad.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLoad.ViewModels
{
    public class SettingsViewModel: ObservableObject
    {
        private IView currentWindow;

        public SettingsViewModel(IView thisWindow)
        {
            this.currentWindow = thisWindow;
        }
    }
}
