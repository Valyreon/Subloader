using System.Threading.Tasks;
using System.Windows;
using SubloaderWpf.ViewModels;

namespace SubloaderWpf.Views;

public partial class TheWindow : Window
{
    public TheWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if(DataContext is TheWindowViewModel viewModel)
        {
            Task.Run(() => viewModel?.Load());
        }
    }
}
