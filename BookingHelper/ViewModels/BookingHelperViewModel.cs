using AutoMapper;
using BookingHelper.DataModels;
using Horizon.Framework.Collections;
using Horizon.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class BookingHelperViewModel : ViewModel
    {
        private readonly IBookingsContext _databaseContext;
        private AttentiveCollection<BookingModel> _bookingContainer;
        private List<BreakRegulation> _breakRegulations;
        private BookingModel _currentBooking;
        private AttentiveCollection<Effort> _efforts;
        private DateTime? _selectedDate;

        public BookingHelperViewModel(IBookingsContext bookingsContext, ICommandFactory commandFactory)
        {
            SaveCommand = commandFactory.CreateCommand(SaveBooking, IsCurrentBookingValid);
            DeleteCommand = commandFactory.CreateCommand<BookingModel>(DeleteBooking);

            _databaseContext = bookingsContext;
            _databaseContext.EnsureDatabaseIsCreated();

            CurrentBooking = new BookingModel();
            SelectedDate = DateTime.Today;
            LoadBookingsForSelectedDate();

            InitializeBreakRegulations();
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

        public ICommand DeleteCommand { get; }

        public AttentiveCollection<Effort> Efforts
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
                OnPropertyChanged(nameof(TotalEffortClearToday));
                OnPropertyChanged(nameof(MandatoryBreakTime));
                OnPropertyChanged(nameof(HomeTime));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public TimeSpan? HomeTime => CalculateEstimatedHomeTime();

        public double MandatoryBreakTime => GetMandatoryBreakTime();

        public ICommand SaveCommand { get; }

        public DateTime? SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                LoadBookingsForSelectedDate();
            }
        }

        public double TotalEffortClearToday => Efforts?.Where(e => !e.Description.ToLower().Contains("pause")).Sum(e => e.EffortTimeInHours) ?? 0;

        public double TotalEffortGrossToday => Efforts?.Sum(e => e.EffortTimeInHours) ?? 0;

        private TimeSpan? CalculateEstimatedHomeTime()
        {
            var startTime = BookingContainer.Min(b => b.StartTime);

            return startTime.HasValue
                ? startTime.Value + TimeSpan.FromHours(8 - TotalEffortClearToday)
                : (TimeSpan?)null;
        }

        private void DeleteBooking(BookingModel booking)
        {
            BookingContainer.Remove(booking);

            _databaseContext.Bookings.Remove(_databaseContext.Bookings.First(b => b.Id == booking.Id));
            _databaseContext.SaveChanges();
        }

        private double GetMandatoryBreakTime()
        {
            double breakTime = 0;
            foreach (var breakRegulation in _breakRegulations.OrderBy(br => br.WorkEffortLimit).ToList())
            {
                if (TotalEffortClearToday > breakRegulation.WorkEffortLimit)
                {
                    breakTime = breakRegulation.MandatoryBreakTime;
                }
                else
                {
                    break;
                }
            }
            return breakTime;
        }

        private void InitializeBreakRegulations()
        {
            _breakRegulations = new List<BreakRegulation>
            {
                new BreakRegulation(6, 0.5),
                new BreakRegulation(9, 0.75)
            };
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

        private IEnumerable<Effort> MemorizeMarkedEfforts()
        {
            return Efforts?.Where(e => e.MarkedAsBooked);
        }

        private AttentiveCollection<Effort> RestoreMarkedEfforts(AttentiveCollection<Effort> efforts, IEnumerable<Effort> markedEfforts)
        {
            if (markedEfforts == null)
            {
                return efforts;
            }

            var comparer = new EffortComparer();
            foreach (var markedEffort in markedEfforts)
            {
                foreach (var effort in efforts)
                {
                    effort.MarkedAsBooked = effort.MarkedAsBooked
                        || comparer.Equals(effort, markedEffort);
                }
            }

            return efforts;
        }

        private void SaveBooking()
        {
            Debug.Assert(SelectedDate.HasValue, "A valid date is a precondition for the command execution.");

            CurrentBooking.Date = SelectedDate.Value;
            var bookingDto = Mapper.Map<Booking>(CurrentBooking);

            _databaseContext.Bookings.Add(bookingDto);
            _databaseContext.SaveChanges();
            CurrentBooking.Id = bookingDto.Id;

            BookingContainer.Add(CurrentBooking);
            CurrentBooking = new BookingModel();
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
            var markedEfforts = MemorizeMarkedEfforts();

            Efforts = new AttentiveCollection<Effort>(BookingContainer
                .GroupBy(b => b.Description)
                .Select(g => new Effort(g.First().Description, g.Sum(b => b.Duration.TotalHours)).RoundEffort(0.25)));

            Efforts = RestoreMarkedEfforts(Efforts, markedEfforts);
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}