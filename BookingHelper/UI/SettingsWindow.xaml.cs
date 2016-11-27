using System.Windows;
using System.Windows.Controls;
using BookingHelper.ViewModels;
using MahApps.Metro;

namespace BookingHelper.UI
{
    internal partial class SettingsWindow
    {
        public SettingsWindow(SettingsViewModel dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }

        private void ChangeTheme(object sender, SelectionChangedEventArgs e)
        {
            var box = (ComboBox) sender;

            ThemeManager.ChangeAppStyle(
                Application.Current, 
                ThemeManager.GetAccent(box.SelectedItem as string),
                ThemeManager.GetAppTheme("BaseDark"));
        }
    }
}