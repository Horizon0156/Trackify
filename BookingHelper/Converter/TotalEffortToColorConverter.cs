using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace BookingHelper.Converter
{
    internal sealed class TotalEffortToColorConverter : IValueConverter
    {
        public object Convert(object value, Type t, object parameter, CultureInfo culture)
        {
            var effort = System.Convert.ToDouble(value);
            var neededEffort = System.Convert.ToDouble(parameter);
            return (effort >= neededEffort) ? Brushes.LimeGreen : Brushes.White;
        }

        public object ConvertBack(object value, Type t, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}