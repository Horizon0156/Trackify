using System;
using System.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Interactivity;

namespace Horizon.Framework.Xaml.Behaviors
{
    public class MinimizeToTrayBehavior : Behavior<Window>
    {
        private NotifyIcon _notificationIcon;        

        protected override void OnAttached()
        {
            base.OnAttached();

            _notificationIcon = new NotifyIcon();
            _notificationIcon.Icon = Icon.ExtractAssociatedIcon(System.Reflection.Assembly.GetEntryAssembly().ManifestModule.Name);
            _notificationIcon.Visible = true;
            _notificationIcon.DoubleClick += MaximizeWindow;

            AssociatedObject.StateChanged += HideTaskbarEntryWhenMinimized;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.StateChanged -= HideTaskbarEntryWhenMinimized;
        }

        private void MaximizeWindow(object sender, EventArgs e)
        {
            AssociatedObject.Show();
            AssociatedObject.WindowState = WindowState.Normal;
        }

        private void HideTaskbarEntryWhenMinimized(object sender, EventArgs e)
        {
            if (AssociatedObject.WindowState == WindowState.Minimized)
            {
                AssociatedObject.Hide();
            }
        }
    }
}
