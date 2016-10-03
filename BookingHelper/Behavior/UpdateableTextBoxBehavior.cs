using MahApps.Metro.Controls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace BookingHelper.Behavior
{
    internal class UpdateableTextBoxBehavior : Behavior<TextBox>
    {
        private bool _isEditingInProgress;
        private string _originalTextBeforeEditing;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.GotFocus += StoreOriginalText;
            AssociatedObject.LostFocus += UpdateBindingsAndStopEditing;
            AssociatedObject.KeyDown += HandleKeyDown;
            AssociatedObject.TextChanged += StartEditing;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.GotFocus += StoreOriginalText;
            AssociatedObject.LostFocus -= UpdateBindingsAndStopEditing;
            AssociatedObject.KeyDown -= HandleKeyDown;
            AssociatedObject.TextChanged -= StartEditing;
        }

        private void CancelEditing()
        {
            if (_isEditingInProgress)
            {
                AssociatedObject.Text = _originalTextBeforeEditing;
                _isEditingInProgress = false;
                StopChangeIndicator();
            }
        }

        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UpdateBindingsAndStopEditing();
            }
            else if (e.Key == Key.Escape)
            {
                CancelEditing();
            }
        }

        private void StartChangeIndicator()
        {
            AssociatedObject.SetValue(TextBoxHelper.IsWaitingForDataProperty, true);
        }

        private void StartEditing(object sender, TextChangedEventArgs e)
        {
            if (!_isEditingInProgress)
            {
                _isEditingInProgress = true;
                StartChangeIndicator();
            }
        }

        private void StopChangeIndicator()
        {
            AssociatedObject.SetValue(TextBoxHelper.IsWaitingForDataProperty, false);
        }

        private void StoreOriginalText(object sender, RoutedEventArgs e)
        {
            if (!_isEditingInProgress)
            {
                _originalTextBeforeEditing = AssociatedObject.Text;
            }
        }

        private void UpdateBindingsAndStopEditing()
        {
            UpdateBindingsIfNecessary();
            StopChangeIndicator();
        }

        private void UpdateBindingsAndStopEditing(object sender, RoutedEventArgs e)
        {
            UpdateBindingsAndStopEditing();
        }

        private void UpdateBindingsIfNecessary()
        {
            if (_isEditingInProgress && !AssociatedObject.Text.Equals(_originalTextBeforeEditing))
            {
                var binding = AssociatedObject.GetBindingExpression(TextBox.TextProperty);
                binding?.UpdateSource();
                _isEditingInProgress = false;
            }
        }
    }
}