using System;
using System.Globalization;
using System.Windows.Data;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public class EqualityToBoolConverter : IValueConverter
    {
        public object Comparand { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, this.Comparand);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
