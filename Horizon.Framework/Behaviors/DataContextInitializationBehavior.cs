using Horizon.Framework.Mvvm;
using System;
using System.Windows;
using System.Windows.Interactivity;

namespace Horizon.Framework.Behaviors
{
    /// <summary>
    /// Behavior which initializes a <see cref="IInitializeable"/> DataContext of a window.
    /// </summary>
    public sealed class DataContextInitializationBehavior : Behavior<Window>
    {
        /// <summary>
        /// Event will be called if an unhandled exception occurs during initialization.
        /// </summary>
        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += InitializeDataContext;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Loaded -= InitializeDataContext;
        }

        private async void InitializeDataContext(object sender, RoutedEventArgs e)
        {
            var initializeableDataContext = AssociatedObject.DataContext as IInitializeable;

            if (initializeableDataContext != null)
            {
                try
                {
                    await initializeableDataContext.InitializeAsync();
                }
                catch (Exception ex)
                {
                    OnUnhandledException(new UnhandledExceptionEventArgs(ex, isTerminating: false));
                }
                finally
                {
                    AssociatedObject.Loaded -= InitializeDataContext;
                }
            }
        }

        private void OnUnhandledException(UnhandledExceptionEventArgs e)
        {
            UnhandledException?.Invoke(this, e);
        }
    }
}