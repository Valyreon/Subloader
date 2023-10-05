using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SubloaderWpf.Views;
/// <summary>
/// Interaction logic for SearchFormControl.xaml
/// </summary>
public partial class SearchFormControl : UserControl
{
    private static readonly Regex numRegex = new(@"^\d*$");

    public SearchFormControl()
    {
        InitializeComponent();

        searchTextBox.AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
        searchTextBox.AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
        searchTextBox.AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);

        RegisterName(searchTextBox.Name, searchTextBox);
    }

    private static void SelectAllText(object sender, RoutedEventArgs e)
    {
        var textBox = e.OriginalSource as TextBox;
        textBox?.SelectAll();
    }

    private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
    {
        // Find the TextBox
        DependencyObject parent = e.OriginalSource as UIElement;
        while (parent is not null and not TextBox)
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        if (parent != null)
        {
            var textBox = (TextBox)parent;
            if (!textBox.IsKeyboardFocusWithin)
            {
                // If the text box is not yet focussed, give it the focus and
                // stop further processing of this click event.
                textBox.Focus();
                e.Handled = true;
            }
        }
    }

    private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        textboxPlaceholder.Visibility = string.IsNullOrWhiteSpace(searchTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
    }

    private async void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        await Task.Delay(25);
        searchTextBox.Focus();
        Keyboard.Focus(searchTextBox);
    }
}
