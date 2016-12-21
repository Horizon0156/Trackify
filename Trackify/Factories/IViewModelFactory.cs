using Trackify.ViewModels;

namespace Trackify.Factories
{
    internal interface IViewModelFactory
    {
        EditTimeAcquisitionViewModel CreateEditTimeAcquisitionViewModel(TimeAcquisitionModel timeAcquisition);

        SettingsViewModel CreateSettingsViewModel();
    }
}