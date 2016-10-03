using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Horizon.Framework.Xaml.Behaviors
{
    public class InputRestrictionBehavior : Behavior<TextBox>
    {
        private static readonly DependencyProperty _inputExpressionProperty = DependencyProperty.Register(
            "InputExpression",
            typeof(string),
            typeof(InputRestrictionBehavior),
            new PropertyMetadata(null));

        public string InputExpression
        {
            get
            {
                return (string)GetValue(_inputExpressionProperty);
            }
            set
            {
                SetValue(_inputExpressionProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewTextInput += ValidateKeyInput;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewTextInput -= ValidateKeyInput;
        }

        private void ValidateKeyInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            var textBox = AssociatedObject;

            var currentText = textBox.Text.Remove(textBox.CaretIndex, textBox.SelectionLength);
            var resultingText = currentText.Insert(textBox.CaretIndex, textCompositionEventArgs.Text);
            var isInputValid = Regex.IsMatch(resultingText, InputExpression);

            textCompositionEventArgs.Handled = !isInputValid;
        }
    }
}