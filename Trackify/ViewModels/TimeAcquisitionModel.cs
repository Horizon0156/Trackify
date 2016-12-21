using Horizon.MvvmFramework.Components;
using System;

namespace Trackify.ViewModels
{
    internal class TimeAcquisitionModel : ObserveableObject, ICloneable
    {
        private string _description;
        private DateTime? _startTime;
        private TimeAcquisitionStateModel _state;
        private DateTime? _stopTime;

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

        public TimeAcquisitionModel Clone()
        {
            return new TimeAcquisitionModel()
            {
                Id = Id,
                StartTime = StartTime,
                StopTime = StopTime,
                Description = Description,
                State = State
            };
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}