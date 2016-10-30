using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using JetBrains.Annotations;

namespace Horizon.Framework.Behaviors
{
    /// <summary>
    /// Behavior which restricts the input of a <see cref="TextBox"/>
    /// </summary>
    public sealed class InputRestrictionBehavior : Behavior<TextBox>
    {
        private static readonly DependencyProperty _inputExpressionProperty = DependencyProperty.Register(
            "InputExpression",
            typeof(string),
            typeof(InputRestrictionBehavior),
            new PropertyMetadata(null));

        /// <summary>
        /// The input restriction as a RegEx expression.
        /// </summary>
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

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewTextInput += ValidateKeyInput;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.PreviewTextInput -= ValidateKeyInput;
        }

        private void ValidateKeyInput([NotNull] object sender, [NotNull] TextCompositionEventArgs textCompositionEventArgs)
        {
            var textBox = AssociatedObject;

            var currentText = textBox.Text.Remove(textBox.CaretIndex, textBox.SelectionLength);
            var resultingText = currentText.Insert(textBox.CaretIndex, textCompositionEventArgs.Text);
            var isInputValid = Regex.IsMatch(resultingText, InputExpression);

            textCompositionEventArgs.Handled = !isInputValid;
        }
    }
}