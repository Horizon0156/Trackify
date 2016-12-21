using AutoMapper;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Components;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Horizon.MvvmFramework.Services;
using Trackify.DataModels;
using Trackify.Messages;

namespace Trackify.ViewModels
{
    internal class EditTimeAcquisitionViewModel : ViewModel
    {
        private readonly IDatabaseContext _dataContext;
        private readonly IMessenger _messenger;
        private DateTime? _referenceDate;
        private TimeSpan? _startTime;
        private TimeSpan? _stopTime;
        private TimeAcquisitionModel _timeAcquisition;

        public EditTimeAcquisitionViewModel(TimeAcquisitionModel timeAcquisition, IDatabaseContext dataContext, IMessenger messenger, ICommandFactory commandFactory)
        {
            _dataContext = dataContext;
            _messenger = messenger;
            IsInEditMode = timeAcquisition != null;
            TimeAcquisition = timeAcquisition ?? new TimeAcquisitionModel() { State = TimeAcquisitionStateModel.Recorded };

            _referenceDate = TimeAcquisition.StartTime?.Date ?? DateTime.Today;
            _startTime = TimeAcquisition.StartTime?.TimeOfDay;
            _stopTime = TimeAcquisition.StopTime?.TimeOfDay;

            SaveCommand = commandFactory.CreateCommand(SaveTimeAcquisition, CanSaveTimeAquisition);
            CancelCommand = commandFactory.CreateCommand(OnClosureRequested);

            TimeAcquisition.PropertyChanged += NotifySaveCommand;
        }

        public ICommand CancelCommand { get; }

        public bool IsInEditMode { get; }

        public DateTime? ReferenceDate
        {
            get
            {
                return _referenceDate;
            }
            set
            {
                SetProperty(ref _referenceDate, value);
                UpdateTimeAcquisition();
            }
        }

        public INotifiableCommand SaveCommand { get; }

        public TimeSpan? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                SetProperty(ref _startTime, value);
                UpdateTimeAcquisition();
            }
        }

        public TimeSpan? StopTime
        {
            get
            {
                return _stopTime;
            }
            set
            {
                SetProperty(ref _stopTime, value);
                UpdateTimeAcquisition();
            }
        }

        public TimeAcquisitionModel TimeAcquisition
        {
            get
            {
                return _timeAcquisition;
            }
            set
            {
                SetProperty(ref _timeAcquisition, value);
            }
        }

        private bool CanSaveTimeAquisition()
        {
            return !string.IsNullOrEmpty(TimeAcquisition.Description)
                   && IsTimeSelectionValid();
        }

        private bool IsTimeSelectionValid()
        {
            if (TimeAcquisition.State == TimeAcquisitionStateModel.Tracking)
            {
                return StartTime.HasValue && StopTime == null;
            }

            return StartTime <= StopTime;
        }

        private void NotifySaveCommand(object sender, PropertyChangedEventArgs e)
        {
            SaveCommand.NotifyChange();
        }

        private void SaveTimeAcquisition()
        {
            TimeAcquisition dto;

            if (IsInEditMode)
            {
                dto = _dataContext.TimeAcquisitions.First(a => a.Id == TimeAcquisition.Id);
                Mapper.Map(TimeAcquisition, dto);
            }
            else
            {
                dto = Mapper.Map<TimeAcquisition>(TimeAcquisition);
                _dataContext.TimeAcquisitions.Add(dto);
            }
            _dataContext.SaveChanges();
            TimeAcquisition.Id = dto.Id;

            _messenger.Send(new DatabaseChangedMessage());
            OnClosureRequested();
        }

        private void UpdateTimeAcquisition()
        {
            TimeAcquisition.StartTime = ReferenceDate + StartTime;
            TimeAcquisition.StopTime = ReferenceDate + StopTime;

            if (TimeAcquisition.StartTime > TimeAcquisition.StopTime)
            {
                TimeAcquisition.StopTime = TimeAcquisition.StopTime?.AddDays(1);
            }
        }
    }
}