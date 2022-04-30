using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SubloaderWpf.Converters
{
    /// <summary>
    ///     Used to convert a boolean to a Visibility class property
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo language)
        {
            if (targetType != typeof(Visibility))
            {
                throw new InvalidOperationException("The target must be a VisibilityProperty");
            }

            var reverse = false;
            if (parameter is string paramStr && !bool.TryParse(paramStr, out reverse))
            {
                throw new ArgumentException("Parameter must be string 'true' or 'false'.");
            }

            if (value is bool boolValue)
            {
                boolValue = reverse ? !boolValue : boolValue;
            }
            else
            {
                return Visibility.Collapsed;
            }

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo language)
        {
            throw new NotImplementedException();
        }
    }
}
