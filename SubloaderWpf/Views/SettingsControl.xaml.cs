using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using SubloaderWpf.ViewModels;

namespace SubloaderWpf.Views;

/// <summary>
/// Interaction logic for SettingsControl.xaml
/// </summary>
public partial class SettingsControl : UserControl
{
    public SettingsControl()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        await Task.Delay(250);
        passwordTextBox.Password = "";
    }

    private void passwordTextBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext != null)
        {
            ((SettingsViewModel)DataContext).Password = ((PasswordBox)sender).Password;
        }
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        searchLanguagesTextBox.Focus();
        Keyboard.Focus(searchLanguagesTextBox);
    }
}
