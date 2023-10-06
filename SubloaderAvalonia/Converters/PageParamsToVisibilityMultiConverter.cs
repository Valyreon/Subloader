using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace SubloaderAvalonia.Converters;
public class PageParamsToVisibilityMultiConverter : IMultiValueConverter
{
    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is int currentPage && values[1] is int totalPages)
        {
            return parameter switch
            {
                null => currentPage > 1 || totalPages > 1,
                bool param => param ? currentPage < totalPages : currentPage > 1,
                _ => false
            };
        }

        return false;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
