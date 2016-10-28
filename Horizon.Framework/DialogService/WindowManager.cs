using System;
using System.Threading.Tasks;
using System.Windows;

using Horizon.Framework.Mvvm;

using JetBrains.Annotations;

namespace Horizon.Framework.DialogService
{
    internal class WindowManager
    {
        [NotNull]
        private readonly ViewModel _responsibleViewModel;

        [NotNull]
        private readonly Window _responsibleWindow;

        [NotNull]
        private readonly TaskCompletionSource<object> _windowPromiseSource;

        public WindowManager([NotNull] Window responsibleWindow, [NotNull] ViewModel responsibleViewModel)
        {
            _windowPromiseSource = new TaskCompletionSource<object>();

            _responsibleWindow = responsibleWindow;
            _responsibleViewModel = responsibleViewModel;

            RegisterEvents();
        }

        [NotNull]
        public Task WindowPromise => _windowPromiseSource.Task;

        public void AttachToMainWindow([NotNull] Window mainWindow)
        {
            if (mainWindow.IsActive)
            {
                _responsibleWindow.Owner = mainWindow;
            }
            else
            {
                mainWindow.Activated += AttachToMainWindowPostOpenend;
            }
        }

        public void ShowBoundWindow()
        {
            _responsibleWindow.DataContext = _responsibleViewModel;
            _responsibleWindow.Show();
        }

        private void AttachToMainWindowPostOpenend([NotNull] object sender, [NotNull] EventArgs eventArgs)
        {
            var mainWindow = (Window)sender;

            _responsibleWindow.Owner = mainWindow;
            mainWindow.Activated -= AttachToMainWindowPostOpenend;
        }

        private void CloseResponsibleWindow([NotNull] object sender, [NotNull] EventArgs e)
        {
            _responsibleWindow.Close();
        }

        private void FulfillWindowPromise([NotNull] object sender, [NotNull] EventArgs e)
        {
            _windowPromiseSource.SetResult(null);
        }

        private void RegisterEvents()
        {
            _responsibleViewModel.ClosureRequested += CloseResponsibleWindow;
            _responsibleWindow.Closed += FulfillWindowPromise;
        }
    }
}