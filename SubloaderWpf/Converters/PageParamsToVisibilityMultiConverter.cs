using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SubloaderWpf.Converters;
public class PageParamsToVisibilityMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is int currentPage && values[1] is int totalPages)
        {
            return parameter switch
            {
                null => currentPage <= 1 && totalPages <= 1 ? Visibility.Collapsed : Visibility.Visible,
                bool param => (param ? currentPage < totalPages : currentPage > 1) ? Visibility.Visible : Visibility.Hidden,
                _ => Visibility.Collapsed
            };
        }

        return Visibility.Collapsed;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
