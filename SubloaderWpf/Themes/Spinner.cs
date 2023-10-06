using System.Windows;
using System.Windows.Controls;

namespace SubloaderWpf.Themes;

public class Spinner : UserControl
{
    static Spinner()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(Spinner),
            new FrameworkPropertyMetadata(typeof(Spinner)));
    }
}
