using BookingHelper.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using BookingHelper.Resources;
using Horizon.Framework.Services;
using log4net;
using MahApps.Metro.Controls.Dialogs;

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

        private void PrefillWithCurrentTimeIfEmpty(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox) sender;

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
            }
        }

        public async void HandleMessageAnnouncement(object sender, MessageEventArgs e)
        {
            var style = e.ButtonSetup == MessageButtonSetup.Affirmitive
                ? MessageDialogStyle.Affirmative
                : MessageDialogStyle.AffirmativeAndNegative;

            var result = await this.ShowMessageAsync(e.Title, e.Message, style);
            e.Result = result == MessageDialogResult.Affirmative
                ? MessageResult.Affirmitive
                : MessageResult.Negative;
        }

        private async void LogExceptionAndTerminateApplication(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Failed to initialize application", e.ExceptionObject as Exception);
            
            await this.ShowMessageAsync("(╯°□°）╯︵ ┻━┻", CultureDependedTexts.InitializationFailure);
            Environment.Exit(-1);
        }
    }
}