using SubLoad.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SubLoad.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class GeneralWindow : Window, IView
    {
        public GeneralWindow()
        {
            this.InitializeComponent();
            this.DataContext = new GeneralWindowViewModel(this);
        }

        public void ChangeCurrentControlTo(object x)
        {
            ((GeneralWindowViewModel)DataContext).CurrentControl = x;
        }
    }
}
