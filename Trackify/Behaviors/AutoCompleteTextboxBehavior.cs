using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Trackify.Behaviors
{
    internal class AutoCompleteTextboxBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty AutoCompletionListProperty = DependencyProperty.Register(
            "AutoCompletionList",
            typeof(IEnumerable<string>),
            typeof(AutoCompleteTextboxBehavior),
            new PropertyMetadata(defaultValue: null));

        public IEnumerable<string> AutoCompletionList
        {
            get
            {
                return (IEnumerable<string>)GetValue(AutoCompletionListProperty);
            }
            set
            {
                SetValue(AutoCompletionListProperty, value);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            EnableAutoCompletion();
        }

        protected override void OnDetaching()
        {
            DisableAutoCompletion();
            base.OnDetaching();
        }

        private void DisableAutoCompletion()
        {
            AssociatedObject.TextChanged -= ProvideAutoCompleteSuggestion;
        }

        private void EnableAutoCompletion()
        {
            AssociatedObject.TextChanged += ProvideAutoCompleteSuggestion;
        }

        private void ProvideAutoCompleteSuggestion(object sender, TextChangedEventArgs e)
        {
            var hasTextBeenAdded = e.Changes.Any(change => change.AddedLength > 0);
            var currentLength = AssociatedObject.Text.Length;

            if (currentLength < 3 || !hasTextBeenAdded)
            {
                return;
            }

            var currentText = AssociatedObject.Text;
            var suggestion = AutoCompletionList?.FirstOrDefault(
                text => text.StartsWith(currentText, StringComparison.InvariantCultureIgnoreCase));

            if (suggestion == null)
            {
                return;
            }

            DisableAutoCompletion();
            AssociatedObject.Text = suggestion;
            AssociatedObject.CaretIndex = currentLength;
            AssociatedObject.SelectionStart = currentLength;
            AssociatedObject.SelectionLength = suggestion.Length - currentLength;
            EnableAutoCompletion();
        }
    }
}