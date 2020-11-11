using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace SubloaderWpf.Converters
{
    public class ColorBrightnessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is null || !(parameter is string))
            {
                parameter = "0";
            }

            if (value is Color color && parameter is string rateStr)
            {
                var success = float.TryParse(rateStr, out var rate);
                return success ? ChangeColorBrightness(color, rate) : value;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is null || !(parameter is string))
            {
                parameter = "1";
            }

            if (value is Color color && parameter is string rateStr)
            {
                var success = float.TryParse(rateStr, out var rate);
                rate = 1 - rate;
                return success ? ChangeColorBrightness(color, rate) : value;
            }

            return null;
        }

        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = ((255 - red) * correctionFactor) + red;
                green = ((255 - green) * correctionFactor) + green;
                blue = ((255 - blue) * correctionFactor) + blue;
            }

            return Color.FromArgb(color.A, (byte)red, (byte)green, (byte)blue);
        }
    }
}
