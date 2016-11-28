using BookingHelper.ViewModels;

namespace BookingHelper.UI
{
    internal partial class SettingsWindow
    {
        public SettingsWindow(SettingsViewModel dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }
    }
}