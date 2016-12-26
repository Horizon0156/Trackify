using Horizon.MvvmFramework.Components;
using System;

namespace Trackify.ViewModels
{
    internal class EditTimeAcquisitionViewModel : ViewModel
    {
        private DateTime? _referenceDate;
        private TimeSpan? _startTime;
        private TimeSpan? _stopTime;
        private TimeAcquisitionModel _timeAcquisition;

        public EditTimeAcquisitionViewModel(TimeAcquisitionModel timeAcquisition)
        {
            TimeAcquisition = timeAcquisition ?? new TimeAcquisitionModel() { State = TimeAcquisitionStateModel.Recorded };

            _referenceDate = TimeAcquisition.StartTime?.Date ?? DateTime.Today;
            _startTime = TimeAcquisition.StartTime?.TimeOfDay;
            _stopTime = TimeAcquisition.StopTime?.TimeOfDay;
        }

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