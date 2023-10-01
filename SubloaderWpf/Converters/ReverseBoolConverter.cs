using System;
using System.Globalization;
using System.Windows.Data;

namespace SubloaderWpf.Converters;

public class ReverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo language)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        throw new ArgumentException("Value must be of type bool.");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
    {
        throw new NotImplementedException();
    }
}
