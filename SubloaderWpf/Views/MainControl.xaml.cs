using System;
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
        searchTextBoxChild ??= searchFormContent.GetFirstDescendantOfType<TextBox>();
        if (searchTextBoxChild != null && e.NewValue is bool visibility && visibility)
        {
            // seems like focus will not work if the control
            // is not drawn (finished processing the visibility change
            await Task.Delay(15);
            searchTextBoxChild.Focus();
        }
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

    private void ListViewItem_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ((MainViewModel)DataContext).Download();
    }

    private void resultsDataGrid_Click(object sender, RoutedEventArgs e)
    {
        var headerClicked = e.OriginalSource as GridViewColumnHeader;

        if (headerClicked.Content is string str)
        {
            if(str == "Release")
            {
                ((MainViewModel)DataContext).SortByRelease();
            }
            else if(str == "Language")
            {
                ((MainViewModel)DataContext).SortByLanguage();
            }
        }
    }
}
