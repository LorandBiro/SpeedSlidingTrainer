using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public class EnumToBrushConverter : IValueConverter
    {
        public List<EnumBrushPair> Items { get; set; } = new List<EnumBrushPair>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Items.First(x => object.Equals(value, x.Key)).Brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Items.First(x => object.Equals(value, x.Brush)).Key;
        }
    }
}
