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
        private readonly IDatabaseContext _databaseContext;
        private readonly IMessenger _messenger;
        private readonly ISettings _settings;
        private readonly Func<SettingsViewModel> _settingsFactory;
        private AttentiveCollection<TimeAcquisitionModel> _timeAcquisitions;
        private TimeAcquisitionModel _currentAcquisition;
        private IEnumerable<Effort> _efforts;
        private DateTime? _selectedDate;

        private bool _isTrackingActive;

        public BookingHelperViewModel(IDatabaseContext databaseContext, ICommandFactory commandFactory, IMessenger messenger, ISettings settings, Func<SettingsViewModel> settingsFactory)
        {
            _databaseContext = databaseContext;
            _commandFactory = commandFactory;
            _messenger = messenger;
            _settings = settings;
            _settingsFactory = settingsFactory;

            ToggleTrackingCommand = commandFactory.CreateCommand(ToggleTracking, CanToggleTracking);
           
            SettingsCommand = commandFactory.CreateCommand(OpenSettings);
            DeleteCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(DeleteBooking);
            
            _messenger.Register<DatabaseChangedMessage>(msg => LoadAcquisitionsForSelectedDate());
            _messenger.Register<BookingTimeIntervalChangedMessage>(msg => UpdateEffort());

            InitializeContent();
        }

        public bool IsTrackingActive
        {
            get
            {
                return _isTrackingActive;
            }
            private set
            {
                SetProperty(ref _isTrackingActive, value);
            }
        }

        private void ToggleTracking()
        {
            IsTrackingActive = !IsTrackingActive;

            PersistCurrentAcquisition();
            ListCurrentAcquisitionProperly();
        }

        public AttentiveCollection<TimeAcquisitionModel> TimeAcquisitions
        {
            get
            {
                return _timeAcquisitions;
            }
            set
            {
                SetProperty(ref _timeAcquisitions, value);
            }
        }

        public TimeAcquisitionModel CurrentAcquisition
        {
            get
            {
                return _currentAcquisition;
            }
            private set
            {
                SetProperty(ref _currentAcquisition, value);
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

        public INotifiableCommand ToggleTrackingCommand { get; }

        public DateTime? SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                SetProperty(ref _selectedDate, value);
                LoadAcquisitionsForSelectedDate();
            }
        }

        public ICommand SettingsCommand { get; }

        public double TotalEffortGrossToday => Efforts?.Sum(e => e.EffortTimeInHours) ?? 0;

        public double TotalEffortNetToday => CalculateNetEffortForToday();

        public void InitializeContent()
        {
            _databaseContext.EnsureDatabaseIsCreated();

            SelectedDate = DateTime.Today;
            InitializeCurrentAcquisition();
        }

        private void InitializeCurrentAcquisition()
        {
            var runningAcquisition = _databaseContext
                .TimeAcquisitions
                .SingleOrDefault(acquisition => acquisition.State == AcquisitionState.Tracking);

            if (runningAcquisition != null)
            {
                CurrentAcquisition = Mapper.Map<TimeAcquisitionModel>(runningAcquisition);
                IsTrackingActive = true;
            }
            else
            {
                CurrentAcquisition = new TimeAcquisitionModel();
            }
            CurrentAcquisition.PropertyChanged += NotifySaveCommand;
        }

        private TimeSpan? CalculateEstimatedHomeTime()
        {
            var startTime = TimeAcquisitions.Min(b => b.StartTime);

            return startTime.HasValue
                ? startTime.Value.TimeOfDay + TimeSpan.FromHours(8 + (TotalEffortGrossToday - TotalEffortNetToday))
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

        private void DeleteBooking(TimeAcquisitionModel timeAcquisition)
        {
            TimeAcquisitions.Remove(timeAcquisition);

            _databaseContext.TimeAcquisitions.Remove(_databaseContext.TimeAcquisitions.First(b => b.Id == timeAcquisition.Id));
            _databaseContext.SaveChanges();
        }

        private bool CanToggleTracking()
        {
            return IsTrackingActive || !string.IsNullOrEmpty(CurrentAcquisition?.Description);
        }

        private void LoadAcquisitionsForSelectedDate()
        {
            if (TimeAcquisitions != null)
            {
                TimeAcquisitions.CollectionChanged -= UpdateEffort;
            }

            if (SelectedDate.HasValue)
            {
                TimeAcquisitions = new AttentiveCollection<TimeAcquisitionModel>(
                    _databaseContext
                        .TimeAcquisitions
                        .Where(b => b.StartTime.HasValue &&  b.StartTime.Value.Date == SelectedDate.Value.Date)
                        .Select(b => Mapper.Map<TimeAcquisitionModel>(b))
                        .OrderBy(b => b.StartTime));

                TimeAcquisitions.FireCollectionChangeWhenInnerElementChanges = true;
                TimeAcquisitions.CollectionChanged += UpdateEffort;
                UpdateEffort();
            }
        }

        private void OpenSettings()
        {
            var settingsModel = _settingsFactory.Invoke();
            _messenger.Send(settingsModel);
        }

        private void PersistCurrentAcquisition()
        {
            Debug.Assert(SelectedDate.HasValue, "A valid date is a precondition for the command execution.");

            CurrentAcquisition.State = IsTrackingActive
                ? TimeAcquisitionStateModel.Tracking
                : TimeAcquisitionStateModel.Recorded;

            var acquisition = _databaseContext.TimeAcquisitions.FirstOrDefault(a => a.Id == CurrentAcquisition.Id);

            if (acquisition != null)
            {
                Mapper.Map(CurrentAcquisition, acquisition);
            }
            else
            {
                acquisition = Mapper.Map<TimeAcquisition>(CurrentAcquisition);
                _databaseContext.TimeAcquisitions.Add(acquisition);
            }
            
            _databaseContext.SaveChanges();
            CurrentAcquisition.Id = acquisition.Id;

            if (!IsTrackingActive)
            {
                TimeAcquisitions.Add(CurrentAcquisition);
                ResetCurrentAcquisition();
            }
        }

        private void ListCurrentAcquisitionProperly()
        {
            if (CurrentAcquisition?.StartTime == null || IsTrackingActive)
            {
                return;
            }

            if (CurrentAcquisition.StartTime.Value.Date == DateTime.Today)
            {
                TimeAcquisitions.Add(CurrentAcquisition);
            }
           
            ResetCurrentAcquisition();
        }

        private void ResetCurrentAcquisition()
        {
            if (CurrentAcquisition != null)
            {
                CurrentAcquisition.PropertyChanged -= NotifySaveCommand;
            }

            CurrentAcquisition = new TimeAcquisitionModel();
            CurrentAcquisition.PropertyChanged += NotifySaveCommand;

            _messenger.Send(new PrepareNewEntryMessage());
        }

        private void NotifySaveCommand(object sender, PropertyChangedEventArgs e)
        {
            ToggleTrackingCommand.NotifyChange();
        }

        private void UpdateEffort()
        {
            Efforts = TimeAcquisitions
                    .GroupBy(b => b.Description)
                    .Select(g => new Effort(_commandFactory, g.ToList()).RoundEffort(_settings.BookingTimeInterval));
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}