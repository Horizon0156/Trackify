using AutoMapper;
using BookingHelper.DataModels;
using Horizon.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private ObservableCollection<BookingModel> _bookingContainer;
        private BookingModel _currentBooking;
        private IBookingsContext _databaseContext;
        private IEnumerable<Effort> _efforts;
        private DateTime? _selectedDate;

        public MainWindowViewModel(IBookingsContext bookingsContext)
        {
            SaveCommand = CreateCommand(SaveBooking, IsCurrentBookingValid);
            DeleteCommand = CreateCommand<BookingModel>(DeleteBooking);

            _databaseContext = bookingsContext;
            _databaseContext.EnsureDatabaseIsCreated();

            CurrentBooking = new BookingModel();
            SelectedDate = DateTime.Today;
            LoadBookingsForSelectedDate();
        }

        public ObservableCollection<BookingModel> BookingContainer
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

        public IEnumerable<Effort> Efforts
        {
            get
            {
                return _efforts;
            }
            private set
            {
                SetProperty(ref _efforts, value);
                OnPropertyChanged("TotalEffortToday");
            }
        }

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

        public double TotalEffortToday => Efforts?.Sum(e => e.EffortTimeInHours) ?? 0;

        private void DeleteBooking(BookingModel booking)
        {
            BookingContainer.Remove(booking);

            _databaseContext.Bookings.Remove(_databaseContext.Bookings.First(b => b.Id == booking.Id));
            _databaseContext.SaveChanges();
        }

        private bool IsCurrentBookingValid()
        {
            return SelectedDate.HasValue && !string.IsNullOrWhiteSpace(CurrentBooking.Description);
        }

        private void LoadBookingsForSelectedDate()
        {
            if (BookingContainer != null)
            {
                BookingContainer.CollectionChanged -= UpdateEffort;
            }

            if (SelectedDate.HasValue)
            {
                BookingContainer = new ObservableCollection<BookingModel>(
                    _databaseContext
                        .Bookings
                        .Where(b => b.Date == SelectedDate)
                        .Select(b => Mapper.Map<BookingModel>(b)));

                BookingContainer.CollectionChanged += UpdateEffort;
                UpdateEffort();
            }
        }

        private void SaveBooking()
        {
            if (!SelectedDate.HasValue)
            {
                throw new InvalidOperationException("The save command can not execute.");
            }

            CurrentBooking.Date = SelectedDate.Value;
            var bookingDto = Mapper.Map<Booking>(CurrentBooking);

            _databaseContext.Bookings.Add(bookingDto);
            _databaseContext.SaveChanges();
            CurrentBooking.Id = bookingDto.Id;

            BookingContainer.Add(CurrentBooking);
            CurrentBooking = new BookingModel();
        }

        private void UpdateEffort()
        {
            Efforts = BookingContainer
                .GroupBy(b => b.Description)
                .Select(g => new Effort(g.First().Description, g.Sum(b => b.Duration.TotalHours)).RoundEffort(0.25));
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}