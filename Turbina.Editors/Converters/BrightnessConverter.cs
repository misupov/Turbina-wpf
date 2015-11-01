using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Turbina.Editors.Converters
{
    public class BrightnessConverter : IValueConverter
    {
        public float Brightness { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;
            if (brush != null)
            {
                return Color.Multiply(brush.Color, Brightness);
            }

            return Color.Multiply((Color)value, Brightness);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}