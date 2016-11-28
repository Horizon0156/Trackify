using AutoMapper;
using BookingHelper.DataModels;
using BookingHelper.Messages;
using BookingHelper.Mocks;
using BookingHelper.Resources;
using Horizon.MvvmFramework.Collections;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Components;
using Horizon.MvvmFramework.Services;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class BookingHelperViewModel : ViewModel
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IBookingsContext _databaseContext;
        private readonly IMessenger _messenger;
        private readonly ISettings _settings;
        private readonly Func<SettingsViewModel> _settingsFactory;
        private AttentiveCollection<BookingModel> _bookingContainer;
        private BookingModel _currentBooking;
        private IEnumerable<Effort> _efforts;
        private DateTime? _selectedDate;

        public BookingHelperViewModel(IBookingsContext bookingsContext, ICommandFactory commandFactory, IMessenger messenger, ISettings settings, Func<SettingsViewModel> settingsFactory)
        {
            _databaseContext = bookingsContext;
            _commandFactory = commandFactory;
            _messenger = messenger;
            _settings = settings;
            _settingsFactory = settingsFactory;

            SaveCommand = commandFactory.CreateCommand(SaveBooking, IsCurrentBookingValid);
            SettingsCommand = commandFactory.CreateCommand(OpenSettings);
            DeleteCommand = commandFactory.CreateCommand<BookingModel>(DeleteBooking);
            
            _messenger.Register<DatabaseChangedMessage>(msg => LoadBookingsForSelectedDate());
            _messenger.Register<BookingTimeIntervalChangedMessage>(msg => UpdateEffort());

            InitializeContent();
        }

        public AttentiveCollection<BookingModel> BookingContainer
        {
            get
            {
                return _bookingContainer;
            }
            set
            {
                SetProperty(ref _bookingContainer, value);
            }
        }

        public BookingModel CurrentBooking
        {
            get
            {
                return _currentBooking;
            }
            private set
            {
                SetProperty(ref _currentBooking, value);
            }
        }

        public INotifiableCommand DeleteCommand { get; }

        public IEnumerable<Effort> Efforts
        {
            get
            {
                return _efforts;
            }
            private set
            {
                SetProperty(ref _efforts, value);

                // ReSharper disable ExplicitCallerInfoArgument, an update of foreign props is desired.
                OnPropertyChanged(nameof(TotalEffortGrossToday));
                OnPropertyChanged(nameof(TotalEffortNetToday));
                OnPropertyChanged(nameof(HomeTime));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public TimeSpan? HomeTime => CalculateEstimatedHomeTime();

        public INotifiableCommand SaveCommand { get; }

        public DateTime? SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                SetProperty(ref _selectedDate, value);
                LoadBookingsForSelectedDate();
            }
        }

        public ICommand SettingsCommand { get; }

        public double TotalEffortGrossToday => Efforts?.Sum(e => e.EffortTimeInHours) ?? 0;

        public double TotalEffortNetToday => CalculateNetEffortForToday();

        public void InitializeContent()
        {
            _databaseContext.EnsureDatabaseIsCreated();

            SelectedDate = DateTime.Today;
            LoadBookingsForSelectedDate();

            PrepareNewBooking();
        }

        private TimeSpan? CalculateEstimatedHomeTime()
        {
            var startTime = BookingContainer.Min(b => b.StartTime);

            return startTime.HasValue
                ? startTime.Value + TimeSpan.FromHours(8 + (TotalEffortGrossToday - TotalEffortNetToday))
                : (TimeSpan?)null;
        }

        private double CalculateNetEffortForToday()
        {
            var breakDeterminationExpression = new Regex($"^({CultureDependedTexts.BreakDescritption})$", RegexOptions.IgnoreCase);

            var netEffort = Efforts?
                .Where(e => !breakDeterminationExpression.IsMatch(e.Description))
                .Sum(e => e.EffortTimeInHours);

            return netEffort ?? 0;
        }

        private void DeleteBooking(BookingModel booking)
        {
            BookingContainer.Remove(booking);

            _databaseContext.Bookings.Remove(_databaseContext.Bookings.First(b => b.Id == booking.Id));
            _databaseContext.SaveChanges();
        }

        private bool IsCurrentBookingValid()
        {
            return SelectedDate.HasValue
                && CurrentBooking.IsBookingEntryValid();
        }

        private void LoadBookingsForSelectedDate()
        {
            if (BookingContainer != null)
            {
                BookingContainer.InnerElementChanged -= SaveChangedBooking;
                BookingContainer.CollectionChanged -= UpdateEffort;
            }

            if (SelectedDate.HasValue)
            {
                BookingContainer = new AttentiveCollection<BookingModel>(
                    _databaseContext
                        .Bookings
                        .Where(b => b.Date == SelectedDate)
                        .Select(b => Mapper.Map<BookingModel>(b)));

                BookingContainer.FireCollectionChangeWhenInnerElementChanges = true;
                BookingContainer.InnerElementChanged += SaveChangedBooking;
                BookingContainer.CollectionChanged += UpdateEffort;
                UpdateEffort();
            }
        }

        private void OpenSettings()
        {
            var settingsModel = _settingsFactory.Invoke();
            _messenger.Send(settingsModel);
        }

        private void SaveBooking()
        {
            Debug.Assert(SelectedDate.HasValue, "A valid date is a precondition for the command execution.");

            // ReSharper disable once PossibleInvalidOperationException Value is availbale
            CurrentBooking.Date = SelectedDate.Value;
            var bookingDto = Mapper.Map<Booking>(CurrentBooking);

            _databaseContext.Bookings.Add(bookingDto);
            _databaseContext.SaveChanges();
            CurrentBooking.Id = bookingDto.Id;

            BookingContainer.Add(CurrentBooking);
            PrepareNewBooking();
        }

        private void PrepareNewBooking()
        {
            if (CurrentBooking != null)
            {
                CurrentBooking.PropertyChanged -= NotifySaveCommand;
            }

            CurrentBooking = new BookingModel();
            CurrentBooking.PropertyChanged += NotifySaveCommand;
            _messenger.Send(new PrepareNewEntryMessage());
        }

        private void NotifySaveCommand(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.NotifyChange();
        }

        private void SaveChangedBooking(object sender, NotifyInnerElementChangedEventArgs e)
        {
            var booking = (BookingModel)e.ChangedItem;

            if (booking.IsBookingEntryValid())
            {
                var bookingToEdit = _databaseContext.Bookings.First(b => b.Id == booking.Id);
                Mapper.Map(booking, bookingToEdit);
                _databaseContext.SaveChanges();
            }
        }

        private void UpdateEffort()
        {
            Efforts = BookingContainer
                    .GroupBy(b => b.Description)
                    .Select(g => new Effort(_commandFactory, g.ToList()).RoundEffort(_settings.BookingTimeInterval));
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}