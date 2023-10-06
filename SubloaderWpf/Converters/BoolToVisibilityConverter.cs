using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SubloaderWpf.Converters;

/// <summary>
///     Used to convert a boolean to a Visibility class property
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is not bool boolValue)
        {
            return Visibility.Collapsed;
        }

        if (parameter is string paramStr && bool.TryParse(paramStr, out var reverse))
        {
            boolValue = reverse ? !boolValue : boolValue;
        }

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException();
    }
}
