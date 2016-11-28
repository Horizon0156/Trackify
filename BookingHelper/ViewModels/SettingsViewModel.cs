using BookingHelper.DataModels;
using BookingHelper.Messages;
using BookingHelper.Mocks;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Components;
using Horizon.MvvmFramework.Services;
using System.Linq;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class SettingsViewModel : ViewModel
    {
        private readonly IBookingsContext _bookingsContext;
        private readonly IMessenger _messenger;
        private readonly IProcess _process;
        private readonly ISettings _settings;
        private int _numberOfBookings;

        public SettingsViewModel(IMessenger messenger, ISettings settings, IBookingsContext bookingsContext, ICommandFactory commandFactory, IProcess process)
        {
            _messenger = messenger;
            _settings = settings;
            _bookingsContext = bookingsContext;
            _process = process;

            _numberOfBookings = bookingsContext.Bookings.Count();
            ResetDatabaseCommand = commandFactory.CreateCommand(ResetDatabase);
            LocateDatabaseCommand = commandFactory.CreateCommand(LocateDatabase);
            ReloadDatabaseCommand = commandFactory.CreateCommand(ReloadDatabase);
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
                _messenger.Send(new BookingTimeIntervalChangedMessage());
            }
        }

        public ICommand LocateDatabaseCommand { get; }

        public int NumberOfBookings
        {
            get
            {
                return _numberOfBookings;
            }
            private set
            {
                SetProperty(ref _numberOfBookings, value);
            }
        }

        public ICommand ReloadDatabaseCommand { get; }

        public ICommand ResetDatabaseCommand { get; }

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

        private void BroadcastAccentColorChange()
        {
            if (SelectedAccentColor != null)
            {
                _messenger.Send(new AccentColorChangedMessage(SelectedAccentColor));
            }
        }

        private void LocateDatabase()
        {
            _process.Start("explorer", _bookingsContext.StorageLocation);
        }

        private void ReloadDatabase()
        {
            _bookingsContext.EnsureDatabaseIsCreated();
            NumberOfBookings = _bookingsContext.Bookings.Count();
            _messenger.Send(new DatabaseChangedMessage());
        }

        private void ResetDatabase()
        {
            _bookingsContext.ResetBookings();
            ReloadDatabase();
        }
    }
}