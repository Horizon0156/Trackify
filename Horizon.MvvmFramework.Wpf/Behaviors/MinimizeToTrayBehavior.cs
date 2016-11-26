using System;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Wpf.Behaviors
{
    /// <summary>
    /// Behavior which minimizes a window into the system's tray.
    /// </summary>
    public sealed class MinimizeToTrayBehavior : Behavior<Window>, IDisposable
    {
        [CanBeNull]
        private NotifyIcon _notificationIcon;

        /// <inheritdoc/>
        public void Dispose()
        {
            _notificationIcon?.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            CreateNotificationIcon();

            AssociatedObject.Closed += HideNotificationIcon;
            AssociatedObject.StateChanged += HideTaskbarEntryWhenMinimized;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.StateChanged -= HideTaskbarEntryWhenMinimized;
            AssociatedObject.Closed -= HideNotificationIcon;
        }

        private void CreateNotificationIcon()
        {
            if (_notificationIcon == null)
            {
                _notificationIcon = new NotifyIcon();
                _notificationIcon.Icon = Icon.ExtractAssociatedIcon(Assembly.GetEntryAssembly().ManifestModule.Name);
                _notificationIcon.Visible = true;
                _notificationIcon.Click += MaximizeWindow;
            }
        }

        private void HideNotificationIcon([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (_notificationIcon != null)
            {
                _notificationIcon.Visible = false;
            }
        }

        private void HideTaskbarEntryWhenMinimized([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Minimized)
            {
                AssociatedObject.Hide();
            }
        }

        private void MaximizeWindow([NotNull] object sender, [NotNull] EventArgs e)
        {
            AssociatedObject.Show();
            AssociatedObject.WindowState = WindowState.Normal;
        }
    }
}