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

    private void LoginButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        InvokeLogin();
    }

    private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        searchLanguagesTextBox.Focus();
        Keyboard.Focus(searchLanguagesTextBox);
    }

    private void NotSelectedLanguages_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((SettingsViewModel)DataContext).Add();
    }

    private void SelectedLanguages_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((SettingsViewModel)DataContext).Delete();
    }

    private void loginTextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Return)
        {
            InvokeLogin();
        }
    }

    private void InvokeLogin()
    {
        var viewModel = (SettingsViewModel)DataContext;
        viewModel.Password = passwordTextBox.Password;
        passwordTextBox.Password = string.Empty;
        viewModel.LoginCommand.Execute(null);
    }
}
