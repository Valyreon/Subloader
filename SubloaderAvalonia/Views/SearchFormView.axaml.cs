using System.Text.RegularExpressions;
using Avalonia.Controls;
using Avalonia.Input;

namespace SubloaderAvalonia.Views;
public partial class SearchFormView : UserControl
{
    private static readonly Regex numRegex = new(@"^\d*$");

    public SearchFormView()
    {
        InitializeComponent();
    }

    private void PreviewNumberInput(object sender, TextInputEventArgs e)
    {
        var isMatch = numRegex.IsMatch(e.Text);
        e.Handled = !isMatch;
    }
}
