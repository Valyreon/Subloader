using System;
using System.Globalization;
using System.Windows.Data;

namespace SubloaderWpf.Converters;

public class MultiplyConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double num && double.TryParse(parameter as string, out var factor))
        {
            var result = num * factor;
            return result;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
