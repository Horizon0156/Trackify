using System;
using System.Windows.Controls;

namespace BookingHelper.UI
{
    internal partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void PrefillWithCurrentTimeIfEmpty(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = string.Format("{0:HH:mm}", DateTime.Now);
            }
        }
    }
}