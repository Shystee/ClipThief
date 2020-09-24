using System;
using System.Globalization;
using System.Windows.Data;

namespace ClipThief.Ui.Converters
{
    public class TimelineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((TimeSpan?)value)?.TotalMilliseconds;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) return TimeSpan.FromMilliseconds((double)value);

            return null;
        }
    }
}