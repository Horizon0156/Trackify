using Trackify.ViewModels;

namespace Trackify.UI
{
    internal partial class EditTimeAcquisitionWindow
    {
        public EditTimeAcquisitionWindow(EditTimeAcquisitionViewModel dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }
    }
}