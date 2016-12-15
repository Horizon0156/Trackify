using BookingHelper.Messages;
using BookingHelper.Resources;
using BookingHelper.ViewModels;
using Horizon.MvvmFramework.Services;
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

        public BookingHelperWindow(BookingHelperViewModel dataContext, ILog logger, IMessenger messenger)
        {
            InitializeComponent();

            _logger = logger;
            DataContext = dataContext;

            messenger.Register<PrepareNewEntryMessage>(PrepareNewEntry);
            messenger.Register<SettingsViewModel>(OpenSettingsWindow);
        }

        private void PrepareNewEntry(PrepareNewEntryMessage message)
        {
            // Todo
        }

        private async void LogExceptionAndTerminateApplication(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Failed to initialize application", e.ExceptionObject as Exception);

            await this.ShowMessageAsync("Uuups :(", CultureDependedTexts.InitializationFailure);
            Environment.Exit(-1);
        }

        private void OpenSettingsWindow(SettingsViewModel settingsModel)
        {
            var settingsWindow = new SettingsWindow(settingsModel);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void PrefillWithCurrentTimeIfEmpty(object sender, RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
            }
        }

        private void OpenTestWindow(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditTimeEntryWindow();
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }
    }
}