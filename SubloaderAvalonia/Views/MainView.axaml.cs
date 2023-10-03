using Avalonia.Controls;

namespace SubloaderAvalonia.Views;

public partial class MainView : UserControl
{
    //private TextBox searchTextBoxChild;

    public MainView()
    {
        InitializeComponent();
    }

    /*private void OnDropFile(object sender, DragEventArgs e)
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
    }*/

    /*private async void searchModal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
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
    }

    private void CancelButton_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus != searchTextBoxChild && searchModal.Visibility == Visibility.Visible)
        {
            searchTextBoxChild.Focus();
            Keyboard.Focus(searchTextBoxChild);
            e.Handled = true;
        }
    }*/
}
