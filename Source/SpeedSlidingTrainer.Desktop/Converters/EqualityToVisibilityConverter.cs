using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public class EqualityToVisibilityConverter : IValueConverter
    {
        public object Comparand { get; set; }

        public Visibility EqualsVisibility { get; set; } = Visibility.Visible;

        public Visibility NotEqualsVisibility { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return object.Equals(value, this.Comparand) ? this.EqualsVisibility : this.NotEqualsVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
