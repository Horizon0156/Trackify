using Horizon.MvvmFramework.Components;
using System;

namespace BookingHelper.ViewModels
{
    internal class TimeAcquisitionModel : ObserveableObject
    {
        private string _description;
        private DateTime? _stopTime;
        private DateTime? _startTime;
        private TimeAcquisitionStateModel _state;

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                SetProperty(ref _description, value);
            }
        }

        public TimeSpan? Duration => StopTime - StartTime;

        public DateTime? StopTime
        {
            get
            {
                return _stopTime;
            }
            set
            {
                SetProperty(ref _stopTime, value);
                OnPropertyChanged("Duration");
            }
        }

        public int Id { get; set; }

        public DateTime? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                SetProperty(ref _startTime, value);
                OnPropertyChanged("Duration");
            }
        }

        public TimeAcquisitionStateModel State
        {
            get
            {
                return _state;
            }
            set
            {
                SetProperty(ref _state, value);
            }
        }
    }
}