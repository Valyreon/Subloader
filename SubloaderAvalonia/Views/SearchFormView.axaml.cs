using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;

namespace SubloaderAvalonia.Views;
public partial class SearchFormView : UserControl
{
    [GeneratedRegex(@"^\d*$")]
    private static partial Regex NumberRegex();

    public SearchFormView()
    {
        InitializeComponent();
    }

    private void PreviewNumberInput(object sender, TextInputEventArgs e)
    {
        var isMatch = NumberRegex().IsMatch(e.Text);
        e.Handled = !isMatch;
    }
}
