using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SubloaderWpf.Extensions;
using SubloaderWpf.ViewModels;

namespace SubloaderWpf.Views;

public partial class MainControl : UserControl
{
    private TextBox searchTextBoxChild;

    public MainControl()
    {
        InitializeComponent();
    }

    private void OnDropFile(object sender, DragEventArgs e)
    {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        ((MainViewModel)DataContext).CurrentPath = files[0];
    }

    private void OnDragOverFile(object sender, DragEventArgs e)
    {
        var files = (string[])e.Data.GetData(DataFormats.FileDrop);

        e.Effects = files.Length == 1 && File.Exists(files[0])
            ? DragDropEffects.Link
            : DragDropEffects.None;

        e.Handled = true;
    }

    private async void searchModal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool visibility && visibility)
        {
            // seems like focus will not work if the control
            // is not drawn (finished processing the visibility change
            await Task.Delay(15);
            searchTextBoxChild.Focus();
        }
    }

    private void TheControl_Loaded(object sender, RoutedEventArgs e)
    {
        searchTextBoxChild = searchFormContent.GetFirstDescendantOfType<TextBox>();

        var viewModel = (MainViewModel)DataContext;

        viewModel?.CheckConnectionCommand?.Execute(null);
    }

    private async void Retry_Button_Click(object sender, RoutedEventArgs e)
    {
        retryButton.IsEnabled = false;
        retrySpinner.Visibility = Visibility.Visible;
        retryButton.Visibility = Visibility.Collapsed;
        retryButton.MoveFocus(new TraversalRequest(FocusNavigationDirection.Up));
        await Task.Delay(2500);
        retryButton.Visibility = Visibility.Visible;
        retrySpinner.Visibility = Visibility.Collapsed;
        retryButton.IsEnabled = true;
    }

    private void CancelButton_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus != searchTextBoxChild && searchModal.Visibility == Visibility.Visible)
        {
            searchTextBoxChild.Focus();
            Keyboard.Focus(searchTextBoxChild);
            e.Handled = true;
        }
    }

    private void retryButton_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (connectionModal.Visibility == Visibility.Visible)
        {
            e.Handled = true;
        }
    }

    private async void connectionModal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is bool visibility && visibility)
        {
            // seems like focus will not work if the control
            // is not drawn (finished processing the visibility change
            await Task.Delay(15);
            retryButton.Focus();
            Keyboard.Focus(retryButton);
        }
    }
}
