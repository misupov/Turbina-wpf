using System;
using System.Globalization;
using System.Windows.Data;

namespace Turbina.Editors.Converters
{
    public class BoolToStringConverter : IValueConverter
    {
        public string FalseValue { get; set; }

        public string TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool?)value).GetValueOrDefault(false) ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return TrueValue.Equals(value);
        }
    }
}