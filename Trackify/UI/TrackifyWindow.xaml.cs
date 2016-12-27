using Horizon.MvvmFramework.Services;
using log4net;
using System;
using System.Windows;
using System.Windows.Input;
using Trackify.Messages;
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
            messenger.Register<EditTimeAcquisitionViewModel>(OpenEditWindow);
        }

        private void LogExceptionAndTerminateApplication(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Failed to initialize application", e.ExceptionObject as Exception);

            Environment.Exit(-1);
        }

        private void MoveFocusToNextElementOnEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var frameworkElement = (FrameworkElement)sender;
                frameworkElement.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            e.Handled = false;
        }

        private void OpenEditWindow(EditTimeAcquisitionViewModel editModel)
        {
            var editWindow = new EditTimeAcquisitionWindow(editModel);
            editWindow.Owner = this;
            editWindow.ShowDialog();
        }

        private void OpenSettingsWindow(SettingsViewModel settingsModel)
        {
            var settingsWindow = new SettingsWindow(settingsModel);
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }

        private void PrepareNewEntry(PrepareNewEntryMessage message)
        {
            DescriptionInput.Focus();
        }
    }
}