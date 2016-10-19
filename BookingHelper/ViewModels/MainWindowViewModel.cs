using AutoMapper;
using BookingHelper.DataModels;
using Horizon.Framework.Mvvm;
using Horizon.Framework.Xaml.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private AttentiveCollection<BookingModel> _bookingContainer;
        private List<BreakRegulation> _breakRegulations;
        private BookingModel _currentBooking;
        private IBookingsContext _databaseContext;
        private IEnumerable<Effort> _efforts;
        private DateTime? _selectedDate;

        public MainWindowViewModel(IBookingsContext bookingsContext)
        {
            SaveCommand = CreateCommand(SaveBooking, IsCurrentBookingValid);
            DeleteCommand = CreateCommand<BookingModel>(DeleteBooking);
            ToggleBookedMarkCommand = CreateCommand<Effort>(ToggleBookedMark);

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

        public ICommand ToggleBookedMarkCommand { get; }

        public IEnumerable<Effort> Efforts
        {
            get
            {
                return _efforts;
            }
            private set
            {
                SetProperty(ref _efforts, value);

                OnPropertyChanged(nameof(TotalEffortGrossToday));
                OnPropertyChanged(nameof(TotalEffortClearToday));
                OnPropertyChanged(nameof(MandatoryBreakTime));
                OnPropertyChanged(nameof(HomeTime));
            }
        }

        public TimeSpan HomeTime => DateTime.Now.TimeOfDay + TimeSpan.FromHours(8 - TotalEffortClearToday);

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

        private void DeleteBooking(BookingModel booking)
        {
            BookingContainer.Remove(booking);

            _databaseContext.Bookings.Remove(_databaseContext.Bookings.First(b => b.Id == booking.Id));
            _databaseContext.SaveChanges();
        }

        private void ToggleBookedMark(Effort effort)
        {
            effort.MarkedAsBooked = !effort.MarkedAsBooked;
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

        private void SaveBooking()
        {
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
            var markedEfforts = Efforts.Where(e => e.MarkedAsBooked);

            Efforts = BookingContainer
                .GroupBy(b => b.Description)
                .Select(g => new Effort(g.First().Description, g.Sum(b => b.Duration.TotalHours)).RoundEffort(0.25));

            if (markedEfforts != null && markedEfforts.Any())
            {
                Efforts = PreserveBookedMarks(Efforts.ToList(), markedEfforts);
            }
        }

        private IEnumerable<Effort> PreserveBookedMarks(List<Effort> efforts, IEnumerable<Effort> oldEfforts)
        {
            var comparer = new EffortComparer();
            foreach(var oldEffort in oldEfforts)
            {
                for(int i=0; i<efforts.Count(); i++)
                {
                    efforts[i].MarkedAsBooked = comparer.Equals(efforts[i], oldEffort);
                }
            }

            return efforts;
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}