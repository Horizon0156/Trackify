using AutoMapper;
using BookingHelper.DataModels;
using BookingHelper.Deployment;
using BookingHelper.Mocks;
using BookingHelper.Resources;
using Horizon.Framework.Collections;
using Horizon.Framework.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class BookingHelperViewModel : ViewModel, IInitializeable
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IBookingsContext _databaseContext;
        private readonly IProcess _process;
        private readonly IUpdateChecker _updateChecker;
        private AttentiveCollection<BookingModel> _bookingContainer;
        private List<BreakRegulation> _breakRegulations;
        private BookingModel _currentBooking;
        private IEnumerable<Effort> _efforts;
        private bool _isUpdateAvailable;
        private DateTime? _selectedDate;

        public BookingHelperViewModel(
            IBookingsContext bookingsContext, ICommandFactory commandFactory, IUpdateChecker updateChecker, IProcess process)
        {
            _databaseContext = bookingsContext;
            _commandFactory = commandFactory;
            _updateChecker = updateChecker;
            _process = process;

            SaveCommand = commandFactory.CreateCommand(SaveBooking, IsCurrentBookingValid);
            DeleteCommand = commandFactory.CreateCommand<BookingModel>(DeleteBooking);
            GetUpdateCommand = commandFactory.CreateCommand(RedirectToApplicationWebsite);
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
                OnPropertyChanged(nameof(MandatoryBreakTime));
                OnPropertyChanged(nameof(HomeTime));
                // ReSharper restore ExplicitCallerInfoArgument
            }
        }

        public ICommand GetUpdateCommand { get; }

        public TimeSpan? HomeTime => CalculateEstimatedHomeTime();

        public bool IsUpdateAvailable
        {
            get
            {
                return _isUpdateAvailable;
            }
            set
            {
                SetProperty(ref _isUpdateAvailable, value);
            }
        }

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
                SetProperty(ref _selectedDate, value);
                LoadBookingsForSelectedDate();
            }
        }

        public double TotalEffortGrossToday => Efforts?.Sum(e => e.EffortTimeInHours) ?? 0;

        public double TotalEffortNetToday => CalculateNetEffortForToday();

        public async Task InitializeAsync()
        {
            _databaseContext.EnsureDatabaseIsCreated();

            CurrentBooking = new BookingModel();

            SelectedDate = DateTime.Today;
            LoadBookingsForSelectedDate();
            InitializeBreakRegulations();
            await CheckForUpdates().ConfigureAwait(false);
        }

        private TimeSpan? CalculateEstimatedHomeTime()
        {
            var startTime = BookingContainer.Min(b => b.StartTime);

            return startTime.HasValue
                ? startTime.Value + TimeSpan.FromHours(8 - (TotalEffortGrossToday - TotalEffortNetToday))
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

        private async Task CheckForUpdates()
        {
            IsUpdateAvailable = await _updateChecker
                .IsUpdateAvailable()
                .ConfigureAwait(true);
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
                if (TotalEffortNetToday > breakRegulation.WorkEffortLimit)
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

        private void RedirectToApplicationWebsite()
        {
            var appWebsite = _updateChecker.ApplicationProductPage;

            if (appWebsite != null)
            {
                _process.Start(appWebsite.AbsoluteUri);
            }
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
            Efforts = BookingContainer
                    .GroupBy(b => b.Description)
                    .Select(g => new Effort(_commandFactory, g.ToList()).RoundEffort(0.25));
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}