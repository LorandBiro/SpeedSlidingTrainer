using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueVisibility { get; set; } = Visibility.Visible;

        public Visibility FalseVisibility { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
            {
                throw new ArgumentException("Value must be boolean.", nameof(value));
            }

            return (bool)value ? this.TrueVisibility : this.FalseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
