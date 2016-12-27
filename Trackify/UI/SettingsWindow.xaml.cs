using Trackify.ViewModels;

namespace Trackify.UI
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