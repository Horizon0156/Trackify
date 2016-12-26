using AutoMapper;
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

using System.Windows.Input;
using Trackify.DataModels;
using Trackify.Factories;
using Trackify.Messages;
using Trackify.Mocks;
using Trackify.Resources;

namespace Trackify.ViewModels
{
    internal class TrackifyViewModel : ViewModel
    {
        private readonly ICommandFactory _commandFactory;
        private readonly IDatabaseContext _databaseContext;
        private readonly IMessenger _messenger;
        private readonly ISettings _settings;
        private readonly IViewModelFactory _viewModelFactory;
        private TimeAcquisitionModel _currentAcquisition;
        private IEnumerable<Effort> _efforts;
        private bool _hasReachedDailyTarget;
        private bool _isTrackingActive;
        private DateTime? _selectedDate;
        private TimeSpan? _targetTime;
        private AttentiveCollection<TimeAcquisitionModel> _timeAcquisitions;
        private double _totalEffort;

        public TrackifyViewModel(IDatabaseContext databaseContext, ICommandFactory commandFactory, IMessenger messenger, ISettings settings, IViewModelFactory viewModelFactory)
        {
            _databaseContext = databaseContext;
            _commandFactory = commandFactory;
            _messenger = messenger;
            _settings = settings;
            _viewModelFactory = viewModelFactory;

            ToggleTrackingCommand = commandFactory.CreateCommand(ToggleTracking);
            SettingsCommand = commandFactory.CreateCommand(OpenSettings);
            DeleteCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(DeleteBooking);
            CreateCommand = commandFactory.CreateCommand(CreateTimeAcquisition);
            EditCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(EditTimeAcquisition, CanEditTimeAcquisition);
            RestartCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(RestartTimeAcquisition);

            _messenger.Register<DatabaseChangedMessage>(msg => LoadAcquisitionsForSelectedDate());
            _settings.PropertyChanged += HandleSettingsUpdate;

            InitializeContent();
        }

        public ICommand CreateCommand { get; set; }

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

        // ReSharper disable once UnusedAutoPropertyAccessor.Global DeleteCommand is bound using BindingProxy
        public ICommand DeleteCommand { get; }

        public ICommand EditCommand { get; }

        public IEnumerable<Effort> Efforts
        {
            get
            {
                return _efforts;
            }
            private set
            {
                SetProperty(ref _efforts, value);
            }
        }

        public bool HasReachedDailyTarget
        {
            get
            {
                return _hasReachedDailyTarget;
            }
            set
            {
                SetProperty(ref _hasReachedDailyTarget, value);
            }
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

        public ICommand RestartCommand { get; }

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

        public TimeSpan? TargetTime
        {
            get
            {
                return _targetTime;
            }
            private set
            {
                SetProperty(ref _targetTime, value);
            }
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

        public INotifiableCommand ToggleTrackingCommand { get; }

        public double TotalEffort
        {
            get
            {
                return _totalEffort;
            }
            private set
            {
                SetProperty(ref _totalEffort, value);
            }
        }

        public void InitializeContent()
        {
            _databaseContext.EnsureDatabaseIsCreated();

            SelectedDate = DateTime.Today;
            InitializeCurrentAcquisition();
        }

        private void CalculateEstimatedTargetTime()
        {
            var startTime = TimeAcquisitions.Min(b => b.StartTime);

            TargetTime = startTime.HasValue
                ? startTime.Value.TimeOfDay + TimeSpan.FromHours(_settings.DailyTarget)
                : (TimeSpan?)null;
        }

        private bool CanEditTimeAcquisition(TimeAcquisitionModel timeAcquisition)
        {
            return timeAcquisition != CurrentAcquisition
                   || IsTrackingActive;
        }

        private void CreateDefaultDescriptionIfNeeded()
        {
            if (string.IsNullOrWhiteSpace(CurrentAcquisition.Description))
            {
                CurrentAcquisition.Description = string.Format(CultureDependedTexts.DefaultDescriptionTemplate, CurrentAcquisition.Id);
            }
        }

        private void CreateTimeAcquisition()
        {
            var creationModel = _viewModelFactory.CreateEditTimeAcquisitionViewModel(timeAcquisition: null);
            _messenger.Send(creationModel);
        }

        private void DeleteBooking(TimeAcquisitionModel timeAcquisition)
        {
            TimeAcquisitions.Remove(timeAcquisition);

            _databaseContext.TimeAcquisitions.Remove(_databaseContext.TimeAcquisitions.First(b => b.Id == timeAcquisition.Id));
            _databaseContext.SaveChanges();
        }

        private void EditTimeAcquisition(TimeAcquisitionModel timeAcquisition)
        {
            var editModel = _viewModelFactory.CreateEditTimeAcquisitionViewModel(timeAcquisition);
            _messenger.Send(editModel);
        }

        private void HandleSettingsUpdate(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.BookingTimeInterval))
            {
                UpdateEffort();
            }
            else if (e.PropertyName == nameof(_settings.DailyTarget))
            {
                CalculateEstimatedTargetTime();
            }
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
            CurrentAcquisition.PropertyChanged += SaveChangedAcquisition;
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

        private void LoadAcquisitionsForSelectedDate()
        {
            if (TimeAcquisitions != null)
            {
                TimeAcquisitions.CollectionChanged -= UpdateEffort;
                TimeAcquisitions.InnerElementChanged -= SaveChangedAcquisition;
            }

            if (SelectedDate.HasValue)
            {
                TimeAcquisitions = new AttentiveCollection<TimeAcquisitionModel>(
                    _databaseContext
                        .TimeAcquisitions
                        .Where(b => (b.State == AcquisitionState.Booked || b.State == AcquisitionState.Recorded) && b.StartTime.Value.Date == SelectedDate.Value.Date)
                        .Select(b => Mapper.Map<TimeAcquisitionModel>(b))
                        .OrderBy(b => b.StartTime));

                TimeAcquisitions.FireCollectionChangeWhenInnerElementChanges = true;
                TimeAcquisitions.CollectionChanged += UpdateEffort;
                TimeAcquisitions.InnerElementChanged += SaveChangedAcquisition;

                UpdateEffort();
            }
        }

        private void OpenSettings()
        {
            var settingsModel = _viewModelFactory.CreateSettingsViewModel();
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
            CreateDefaultDescriptionIfNeeded();
        }

        private void ResetCurrentAcquisition()
        {
            if (CurrentAcquisition != null)
            {
                CurrentAcquisition.PropertyChanged -= SaveChangedAcquisition;
            }

            CurrentAcquisition = new TimeAcquisitionModel();
            CurrentAcquisition.PropertyChanged += SaveChangedAcquisition;

            _messenger.Send(new PrepareNewEntryMessage());
        }

        private void RestartTimeAcquisition(TimeAcquisitionModel timeAcquisition)
        {
            if (IsTrackingActive)
            {
                ToggleTracking();
            }

            CurrentAcquisition.Description = timeAcquisition.Description;
            IsTrackingActive = true;
        }

        private void SaveChangedAcquisition(object sender, PropertyChangedEventArgs e)
        {
            SaveChangedAcquisition((TimeAcquisitionModel)sender);
        }

        private void SaveChangedAcquisition(TimeAcquisitionModel changedAcquisition)
        {
            var acquisitionToEdit = _databaseContext
                .TimeAcquisitions
                .FirstOrDefault(b => b.Id == changedAcquisition.Id);

            if (acquisitionToEdit != null)
            {
                Mapper.Map(changedAcquisition, acquisitionToEdit);
                _databaseContext.SaveChanges();
            }
        }

        private void SaveChangedAcquisition(object sender, NotifyInnerElementChangedEventArgs e)
        {
            SaveChangedAcquisition((TimeAcquisitionModel)e.ChangedItem);
        }

        private void ToggleTracking()
        {
            IsTrackingActive = !IsTrackingActive;

            PersistCurrentAcquisition();
            ListCurrentAcquisitionProperly();
        }

        private void UpdateEffort()
        {
            Efforts = TimeAcquisitions
                    .GroupBy(b => b.Description)
                    .Select(g => new Effort(_commandFactory, g.ToList()).RoundEffort(_settings.BookingTimeInterval));

            TotalEffort = Efforts.Sum(e => e.EffortTimeInHours);
            HasReachedDailyTarget = TotalEffort >= _settings.DailyTarget;

            CalculateEstimatedTargetTime();
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}