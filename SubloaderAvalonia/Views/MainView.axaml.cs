using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using SubloaderAvalonia.ViewModels;

namespace SubloaderAvalonia.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        Loaded += MainView_Loaded;
        openSearchModalButton.Click += OpenSearchModalButton_Click;
    }

    private async void OpenSearchModalButton_Click(object sender, RoutedEventArgs e)
    {
        foreach(var i in searchFormContent.GetVisualDescendants())
        {
            if(i is TextBox box)
            {
                await Task.Delay(25);
                box.Focus();
                return;
            }
        }
    }

    private void MainView_Loaded(object sender, RoutedEventArgs e)
    {
        
    }

    private void ListBoxItem_DoubleTapped(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Button Pressed");
    }

    private void TheControl_Loaded(object sender, RoutedEventArgs e)
    {
        //listBox.DoubleTapped += Handle_ListBox_DoubleTap;
    }

    private void Handle_ListBox_DoubleTap(object sender, TappedEventArgs e)
    {
        ((MainViewModel)DataContext).Download();
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
    }*/

    /*private void CancelButton_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (e.NewFocus != searchTextBoxChild && searchModal.Visibility == Visibility.Visible)
        {
            searchTextBoxChild.Focus();
            Keyboard.Focus(searchTextBoxChild);
            e.Handled = true;
        }
    }*/
}
