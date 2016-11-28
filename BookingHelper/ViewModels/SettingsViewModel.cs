using BookingHelper.Messages;
using BookingHelper.Mocks;
using Horizon.MvvmFramework.Components;
using Horizon.MvvmFramework.Services;

namespace BookingHelper.ViewModels
{
    internal class SettingsViewModel : ViewModel
    {
        private readonly IMessenger _messenger;
        private readonly ISettings _settings;

        public SettingsViewModel(IMessenger messenger, ISettings settings)
        {
            _messenger = messenger;
            _settings = settings;
        }

        public string SelectedAccentColor
        {
            get
            {
                return _settings.AccentColor;
            }
            set
            {
                _settings.AccentColor = value;
                OnPropertyChanged();
                BroadcastAccentColorChange();
            }
        }

        public double BookingTimeInterval
        {
            get
            {
                return _settings.BookingTimeInterval;
            }
            set
            {
                _settings.BookingTimeInterval = value;
            }
        }

        private void BroadcastAccentColorChange()
        {
            if (SelectedAccentColor != null)
            {
                _messenger.Send(new AccentColorChangedMessage(SelectedAccentColor));
            }
        }
    }
}