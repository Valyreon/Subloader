using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SubloaderWpf.Converters;

/// <summary>
///     Used to convert a boolean to a Visibility class property
/// </summary>
public class ObjectToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (targetType != typeof(Visibility))
        {
            throw new InvalidOperationException("The target must be a VisibilityProperty");
        }

        return value == null ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException();
    }
}
