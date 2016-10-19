using Horizon.Framework.Mvvm;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace Horizon.Framework.DialogService
{
    /// <summary>
    /// Implementation of a dialog service.
    /// </summary>
    public class DialogService : IDialogService
    {
        [NotNull]
        private readonly List<WindowManager> _activeWindowManager;

        [NotNull]
        private readonly Dictionary<Type, Type> _registeredWindowTypesByViewModelType;

        [CanBeNull]
        private Func<Type, Window> _customWindowActivator;

        [CanBeNull]
        private Window _mainWindow;

        /// <summary>
        /// Creates a new dialog service for the given main window.
        /// </summary>
        public DialogService()
        {
            _activeWindowManager = new List<WindowManager>();
            _registeredWindowTypesByViewModelType = new Dictionary<Type, Type>();
        }

        /// <summary>
        /// Attaches this service to the main window.
        /// Used for handling topmost vsiblity, closures of sub windows, etc.
        /// </summary>
        /// <param name="window"></param>
        public void RegisterMainWindow([NotNull] Window window)
        {
            _mainWindow = window;
            foreach (var windowManager in _activeWindowManager)
            {
                windowManager.AttachToMainWindow(window);
            }
        }

        /// <summary>
        /// Registers a custom window activator (e.g. to use as DI container for window creation).
        /// </summary>
        /// <param name="windowActivator"> The window activator. </param>
        public void RegisterCustomWindowActivator(Func<Type, Window> windowActivator)
        {
            _customWindowActivator = windowActivator;
        }

        /// <summary>
        /// Registers a dialog view model to a corresponding window.
        /// </summary>
        /// <typeparam name="TV"> The type of the viewmodel. </typeparam>
        /// <typeparam name="TW"> The type of the window. </typeparam>
        public void RegisterDialog<TV, TW>() where TV : ViewModel where TW : Window
        {
            var windowType = typeof(TW);
            var vmType = typeof(TV);

            _registeredWindowTypesByViewModelType.Add(vmType, windowType);
        }

        /// <inheritdoc/>
        public void ShowDialog(ViewModel viewModel)
        {
            var manager = CreateWindowManager(viewModel);
            manager.OpenBoundWindow();
        }

        /// <inheritdoc/>
        public Task ShowModalDialog(ViewModel viewModel)
        {
            var manager = CreateWindowManager(viewModel);
            manager.OpenBoundWindow();

            return manager.WindowPromise;
        }

        [NotNull]
        private Window CreateWindow([NotNull] Type viewModelType)
        {
            if (!_registeredWindowTypesByViewModelType.ContainsKey(viewModelType))
            {
                throw new InvalidOperationException($"Window for view model of type {viewModelType} is not registered");
            }
            var windowType = _registeredWindowTypesByViewModelType[viewModelType];

            var window = _customWindowActivator != null
                             ? _customWindowActivator.Invoke(windowType)
                             : (Window)Activator.CreateInstance(windowType);

            return window;
        }

        [NotNull]
        private WindowManager CreateWindowManager([NotNull] ViewModel viewModel)
        {
            var window = CreateWindow(viewModel.GetType());
            var manager = new WindowManager(window, viewModel);

            if (_mainWindow != null)
            {
                manager.AttachToMainWindow(_mainWindow);
            }

            _activeWindowManager.Add(manager);
            manager.WindowPromise.ContinueWith(t => DropManager(manager));

            return manager;
        }

        private void DropManager([NotNull] WindowManager manager)
        {
            _activeWindowManager.Remove(manager);
        }
    }
}