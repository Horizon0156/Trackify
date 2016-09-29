using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BookingHelper.Controls
{
    internal class TimeBox : TextBox
    {
        private const string TIME_INPUT_PATTERN = "^[0-2]?[0-9]?:?[0-9]?[0-9]?$";

        private const string TIME_VALIDATION_PATTERN = "^[0-2]?[0-9]:?[0-9][0-9]$";

        private static readonly DependencyProperty _timeProperty = DependencyProperty.Register(
            "Time",
            typeof(TimeSpan?),
            typeof(TimeBox),
            new FrameworkPropertyMetadata(null, ConvertTimeToText));

        public TimeSpan? Time
        {
            get
            {
                return (TimeSpan?)GetValue(_timeProperty);
            }
            set
            {
                SetValue(_timeProperty, value);
            }
        }

        public TimeBox()
        {
            PreviewTextInput += ValidateInput;
        }

        private void ValidateInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            var currentText = Text.Remove(CaretIndex, SelectionLength);
            var resultingText = currentText.Insert(CaretIndex, textCompositionEventArgs.Text);
            var isInputValid = Regex.IsMatch(resultingText, TIME_INPUT_PATTERN);
            textCompositionEventArgs.Handled = !isInputValid;
        }

        private static object ConvertTimeToText(DependencyObject d, object basevalue)
        {
            return d;
        }
    }
}