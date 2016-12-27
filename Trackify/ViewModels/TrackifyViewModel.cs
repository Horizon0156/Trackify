using AutoMapper;
using Horizon.MvvmFramework.Collections;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Components;
using Horizon.MvvmFramework.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
        private readonly IViewModelFactory _viewModelFactory;
        private ObservableCollection<string> _autoCompletionValues;
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
            _viewModelFactory = viewModelFactory;

            ToggleTrackingCommand = commandFactory.CreateCommand(ToggleTracking);
            SettingsCommand = commandFactory.CreateCommand(OpenSettings);
            DeleteCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(DeleteAcquisition);
            CreateCommand = commandFactory.CreateCommand(CreateTimeAcquisition);
            EditAcquisitionCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(EditTimeAcquisition);
            EditCurrentAcquisitionCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(EditTimeAcquisition, CanEditCurrentTimeAcquisition);
            RestartCommand = commandFactory.CreateCommand<TimeAcquisitionModel>(RestartTimeAcquisition);

            _messenger.Register<DatabaseChangedMessage>(msg => LoadAcquisitionsForSelectedDate());

            Settings = settings;
            Settings.PropertyChanged += HandleSettingsUpdate;

            InitializeContent();
        }

        public ObservableCollection<string> AutoCompletionValues
        {
            get
            {
                return _autoCompletionValues;
            }
            private set
            {
                SetProperty(ref _autoCompletionValues, value);
            }
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

        public ICommand EditAcquisitionCommand { get; }

        public INotifiableCommand EditCurrentAcquisitionCommand { get; }

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
                EditCurrentAcquisitionCommand.NotifyChange();
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

        public ISettings Settings { get; }

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

            PrepareAutocompletion();
            SelectedDate = DateTime.Today;
            InitializeCurrentAcquisition();
        }

        private void CalculateEstimatedTargetTime()
        {
            var startTime = TimeAcquisitions.Min(b => b.StartTime);

            TargetTime = startTime.HasValue
                ? startTime.Value.TimeOfDay + TimeSpan.FromHours(Settings.DailyTarget)
                : (TimeSpan?)null;
        }

        private bool CanEditCurrentTimeAcquisition(TimeAcquisitionModel timeAcquisition)
        {
            return IsTrackingActive;
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
            var currentTime = DateTime.Now;
            var preparedAcquisition = new TimeAcquisitionModel
            {
                State = TimeAcquisitionStateModel.Recorded,
                StartTime = currentTime,
                StopTime = currentTime
            };

            var editModel = _viewModelFactory.CreateEditTimeAcquisitionViewModel(preparedAcquisition);
            _messenger.Send(editModel);

            var createdAcquisition = editModel.TimeAcquisition;
            if (createdAcquisition.IsValid())
            {
                PersistChangesToAcquisition(createdAcquisition);
                ListAcquisitionProperly(createdAcquisition);
            }
        }

        private void DeleteAcquisition(TimeAcquisitionModel timeAcquisition)
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
            if (e.PropertyName == nameof(Settings.BookingTimeInterval))
            {
                UpdateEffort();
            }
            else if (e.PropertyName == nameof(Settings.DailyTarget))
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
            CurrentAcquisition.PropertyChanged += PersistChangesToAcquisition;
        }

        private void ListAcquisitionProperly(TimeAcquisitionModel acquisition)
        {
            if (SelectedDate.HasValue
                && acquisition.StartTime.HasValue
                && acquisition.StartTime.Value.Date == SelectedDate.Value)
            {
                TimeAcquisitions.Add(acquisition);
            }
        }

        private void ListCurrentAcquisitionProperly()
        {
            if (IsTrackingActive)
            {
                return;
            }

            ListAcquisitionProperly(CurrentAcquisition);
            RememberDescriptionForAutoCompletion();
            ResetCurrentAcquisition();
        }

        private void LoadAcquisitionsForSelectedDate()
        {
            if (TimeAcquisitions != null)
            {
                TimeAcquisitions.CollectionChanged -= UpdateEffort;
                TimeAcquisitions.InnerElementChanged -= PersistChangesToAcquisition;
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
                TimeAcquisitions.InnerElementChanged += PersistChangesToAcquisition;

                UpdateEffort();
            }
        }

        private void OpenSettings()
        {
            var settingsModel = _viewModelFactory.CreateSettingsViewModel();
            _messenger.Send(settingsModel);
        }

        private void PersistChangesToAcquisition(object sender, PropertyChangedEventArgs e)
        {
            PersistChangesToAcquisition((TimeAcquisitionModel)sender);
        }

        private void PersistChangesToAcquisition(TimeAcquisitionModel acquisition)
        {
            if (acquisition.State == TimeAcquisitionStateModel.Initialized)
            {
                return;
            }

            var acquisitionToEdit = _databaseContext
                .TimeAcquisitions
                .FirstOrDefault(entry => entry.Id == acquisition.Id);

            if (acquisitionToEdit != null)
            {
                Mapper.Map(acquisition, acquisitionToEdit);
                _databaseContext.SaveChanges();
            }
            else
            {
                acquisitionToEdit = Mapper.Map<TimeAcquisition>(acquisition);
                _databaseContext.TimeAcquisitions.Add(acquisitionToEdit);
                _databaseContext.SaveChanges();
                acquisition.Id = acquisitionToEdit.Id;
            }
        }

        private void PersistChangesToAcquisition(object sender, NotifyInnerElementChangedEventArgs e)
        {
            PersistChangesToAcquisition((TimeAcquisitionModel)e.ChangedItem);
        }

        private void PrepareAutocompletion()
        {
            AutoCompletionValues = new ObservableCollection<string>(
                _databaseContext
                    .TimeAcquisitions
                    .OrderByDescending(a => a.StartTime)
                    .Select(a => a.Description)
                    .Distinct()
                    .Take(50));
        }

        private void RememberDescriptionForAutoCompletion()
        {
            if (!AutoCompletionValues.Contains(CurrentAcquisition.Description))
            {
                AutoCompletionValues.Add(CurrentAcquisition.Description);
            }
        }

        private void ResetCurrentAcquisition()
        {
            if (CurrentAcquisition != null)
            {
                CurrentAcquisition.PropertyChanged -= PersistChangesToAcquisition;
            }

            CurrentAcquisition = new TimeAcquisitionModel();
            CurrentAcquisition.PropertyChanged += PersistChangesToAcquisition;

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

        private void ToggleTracking()
        {
            IsTrackingActive = !IsTrackingActive;

            CurrentAcquisition.State = IsTrackingActive
                ? TimeAcquisitionStateModel.Tracking
                : TimeAcquisitionStateModel.Recorded;
            CreateDefaultDescriptionIfNeeded();

            ListCurrentAcquisitionProperly();
        }

        private void UpdateEffort()
        {
            Efforts = TimeAcquisitions
                    .GroupBy(b => b.Description)
                    .Select(g => new Effort(_commandFactory, g.ToList()).RoundEffort(Settings.BookingTimeInterval));

            TotalEffort = Efforts.Sum(e => e.EffortTimeInHours);
            HasReachedDailyTarget = TotalEffort >= Settings.DailyTarget;

            CalculateEstimatedTargetTime();
        }

        private void UpdateEffort(object sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateEffort();
        }
    }
}