using System.IO;
using System.Windows;
using System.Windows.Controls;
using SubloaderWpf.ViewModels;

namespace SubloaderWpf.Views;

public partial class MainControl : UserControl
{
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

    private void searchModal_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if(e.NewValue is bool visibility && visibility)
        {
            searchTextBox.SelectAll();
            searchTextBox.Focus();
        }
    }
}
