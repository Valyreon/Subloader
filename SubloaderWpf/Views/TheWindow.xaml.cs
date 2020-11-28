using System.Windows;
using SubloaderWpf.ViewModels;

namespace SubloaderWpf.Views
{
    public partial class TheWindow : Window
    {
        public TheWindow()
        {
            InitializeComponent();
            DataContext = new TheWindowViewModel();
        }
    }
}
