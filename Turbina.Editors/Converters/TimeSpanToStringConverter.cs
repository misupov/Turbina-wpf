using System;
using System.Globalization;
using System.Windows.Data;

namespace Turbina.Editors.Converters
{
    public class TimeSpanToStringConverter : IValueConverter
    {
        public string FalseValue { get; set; }

        public string TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as TimeSpan?)?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (value as string);
            if (s != null)
            {
                var result = TimeSpan.Zero;
                TimeSpan.TryParse(s, out result);
                return result;
            }

            return null;
        }
    }
}