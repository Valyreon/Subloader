using SubloaderWpf.ViewModels;
using System.Windows;

namespace SubloaderWpf.Views
{
    /// <summary>
    /// Interaction logic for GeneralWindow.xaml
    /// </summary>
    public partial class TheWindow : Window
    {
        public TheWindow()
        {
            this.InitializeComponent();
            this.DataContext = new TheWindowViewModel();
        }
    }
}
