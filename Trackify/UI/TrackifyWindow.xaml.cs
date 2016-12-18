using Horizon.MvvmFramework.Services;
using log4net;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;
using Trackify.Messages;
using Trackify.Resources;
using Trackify.ViewModels;

namespace Trackify.UI
{
    internal partial class TrackifyWindow
    {
        private readonly ILog _logger;

        public TrackifyWindow(TrackifyViewModel dataContext, ILog logger, IMessenger messenger)
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

        private void OpenTestWindow(object sender, RoutedEventArgs e)
        {
            var editWindow = new EditTimeEntryWindow();
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }
    }
}