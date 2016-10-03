using System;
using System.Globalization;
using System.Windows.Data;

namespace BookingHelper.Converter
{
    internal class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var timespan = value as TimeSpan?;

            return timespan?.ToString(@"hh\:mm", CultureInfo.InvariantCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;

            if (!string.IsNullOrEmpty(text))
            {
                TimeSpan parsedTime;

                if (TryParseTextUsingHourMinueteFormat(text, out parsedTime))
                {
                    return parsedTime;
                }
                else if (TimeSpan.TryParse(text, out parsedTime))
                {
                    return parsedTime;
                }
            }

            return null;
        }

        private void EnsureTimeDelimeterIsPresent(ref string text)
        {
            var minuetStartIndex = text.Length - 2;

            if (minuetStartIndex > 0 && !text.Contains(":"))
            {
                text = text.Insert(minuetStartIndex, ":");
            }
        }

        private bool TryParseTextUsingHourMinueteFormat(string text, out TimeSpan parsedTime)
        {
            EnsureTimeDelimeterIsPresent(ref text);

            return TimeSpan.TryParseExact(text, @"HH\:mm", CultureInfo.InvariantCulture, out parsedTime);
        }
    }
}