using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SpeedSlidingTrainer.Desktop.Converters
{
    public sealed class SwitchConverter : IValueConverter
    {
        private static readonly List<KeyValuePair<string, object>> ConstantValues = new List<KeyValuePair<string, object>>
        {
            new KeyValuePair<string, object>("null", null),
            new KeyValuePair<string, object>("true", true),
            new KeyValuePair<string, object>("false", false),
            new KeyValuePair<string, object>("Visible", Visibility.Visible),
            new KeyValuePair<string, object>("Hidden", Visibility.Hidden),
            new KeyValuePair<string, object>("Collapsed", Visibility.Collapsed)
        };

        private static readonly Dictionary<string, Switch> SwitchCache = new Dictionary<string, Switch>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string switchString = parameter as string;
            if (switchString == null)
            {
                throw new ArgumentException("Converter parameter must be a string.");
            }

            Switch @switch;
            if (!SwitchCache.TryGetValue(switchString, out @switch))
            {
                @switch = ParseSwitch(switchString);
                SwitchCache.Add(switchString, @switch);
            }

            string valueString = value == null ? "null" : value.ToString();
            foreach (KeyValuePair<string, object> @case in @switch.Cases)
            {
                if (@case.Key == valueString)
                {
                    return @case.Value;
                }
            }

            return @switch.DefaultValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Switch converters cannot convert values back.");
        }

        private static Switch ParseSwitch(string switchString)
        {
            if (string.IsNullOrEmpty(switchString))
            {
                throw new SwitchConverterException("Switch expression cannot be empty.");
            }

            Switch @switch = new Switch();

            List<string> caseStrings = switchString.Split(';').Select(caseString => caseString.Trim()).ToList();
            if (!caseStrings[caseStrings.Count - 1].Contains(":"))
            {
                @switch.DefaultValue = ParseValue(caseStrings[caseStrings.Count - 1]);
                caseStrings.RemoveAt(caseStrings.Count - 1);
            }

            foreach (string caseString in caseStrings)
            {
                @switch.Cases.Add(ParseCase(caseString));
            }

            return @switch;
        }

        private static KeyValuePair<string, object> ParseCase(string caseString)
        {
            if (string.IsNullOrEmpty(caseString))
            {
                throw new SwitchConverterException("Case expression cannot be empty.");
            }

            List<string> parts = caseString.Split(':').Select(x => x.Trim()).ToList();
            if (parts.Count != 2)
            {
                throw new SwitchConverterException("A case expression must contain exactly 1 colon.");
            }

            return new KeyValuePair<string, object>(parts[0], ParseValue(parts[1]));
        }

        private static object ParseValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new SwitchConverterException("Value cannot be empty.");
            }

            foreach (KeyValuePair<string, object> constantValue in ConstantValues)
            {
                if (value == constantValue.Key)
                {
                    return constantValue.Value;
                }
            }

            return (SolidColorBrush)new BrushConverter().ConvertFrom(value);
        }

        private class Switch
        {
            public List<KeyValuePair<string, object>> Cases { get; } = new List<KeyValuePair<string, object>>();

            public object DefaultValue { get; set; }
        }
    }
}
