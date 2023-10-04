using System.Windows;
using System.Windows.Media;

namespace SubloaderWpf.Extensions;
public static class VisualTreeExtensions
{
    public static T GetFirstDescendantOfType<T>(this DependencyObject parent) where T : DependencyObject
    {
        // Get the number of child elements.
        var count = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < count; ++i)
        {
            // Get the child element at index i.
            var child = VisualTreeHelper.GetChild(parent, i);

            // Check if the child element is of the desired type.
            if (child is T typedChild)
            {
                return typedChild;
            }

            // Recursively search for child elements.
            var x = GetFirstDescendantOfType<T>(child);
            if(x != null)
            {
                return x;
            }
        }

        return null;
    }
}
