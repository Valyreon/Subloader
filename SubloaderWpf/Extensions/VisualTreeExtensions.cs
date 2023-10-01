using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace SubloaderWpf.Extensions;
public static class VisualTreeExtensions
{
    // Define an extension method to get all child elements of a certain type.
    public static List<T> GetChildrenOfType<T>(this DependencyObject parent) where T : DependencyObject
    {
        var children = new List<T>(5);

        // Get the number of child elements.
        var count = VisualTreeHelper.GetChildrenCount(parent);

        for (var i = 0; i < count; i++)
        {
            // Get the child element at index i.
            var child = VisualTreeHelper.GetChild(parent, i);

            // Check if the child element is of the desired type.
            if (child is T typedChild)
            {
                children.Add(typedChild);
            }

            // Recursively search for child elements.
            children.AddRange(GetChildrenOfType<T>(child));
        }

        return children;
    }

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
