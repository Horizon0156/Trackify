using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interactivity;
using JetBrains.Annotations;

namespace Horizon.Framework.Xaml.Behaviors
{
    /// <summary>
    /// Behavior which minimizes a window into the system's tray.
    /// </summary>
    public sealed class MinimizeToTrayBehavior : Behavior<Window>
    {
        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            var notificationIcon = new NotifyIcon();
            notificationIcon.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            notificationIcon.Visible = true;
            notificationIcon.DoubleClick += MaximizeWindow;

            AssociatedObject.StateChanged += HideTaskbarEntryWhenMinimized;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.StateChanged -= HideTaskbarEntryWhenMinimized;
        }

        private void MaximizeWindow([NotNull] object sender, [NotNull] EventArgs e)
        {
            AssociatedObject.Show();
            AssociatedObject.WindowState = WindowState.Normal;
        }

        private void HideTaskbarEntryWhenMinimized([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Minimized)
            {
                AssociatedObject.Hide();
            }
        }
    }
}
