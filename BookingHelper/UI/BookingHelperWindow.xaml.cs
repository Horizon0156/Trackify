using BookingHelper.Messages;
using BookingHelper.Resources;
using BookingHelper.ViewModels;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BookingHelper.UI
{
    internal partial class BookingHelperWindow
    {
        private readonly ILog _logger;

        public BookingHelperWindow(BookingHelperViewModel dataContext, ILog logger)
        {
            InitializeComponent();

            _logger = logger;
            DataContext = dataContext;
        }

        public void PrepareNewEntry(PrepareNewEntryMessage message)
        {
            StartTimeBox.Focus();
        }

        private async void LogExceptionAndTerminateApplication(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Failed to initialize application", e.ExceptionObject as Exception);

            await this.ShowMessageAsync("(╯°□°）╯︵ ┻━┻", CultureDependedTexts.InitializationFailure);
            Environment.Exit(-1);
        }

        private void PrefillWithCurrentTimeIfEmpty(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
            }
        }
    }
}