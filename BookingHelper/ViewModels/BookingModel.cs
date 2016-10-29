using Horizon.Framework.Mvvm;
using System;

namespace BookingHelper.ViewModels
{
    internal class BookingModel : ObserveableObject
    {
        private string _description;
        private TimeSpan? _endTime;
        private TimeSpan? _startTime;
        private BookingModelState _state;

        public DateTime Date { get; set; }

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

        public TimeSpan Duration => CalculateDurationBasedOnStartAndEndTime();

        public TimeSpan? EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                SetProperty(ref _endTime, value);
            }
        }

        public int Id { get; set; }

        public TimeSpan? StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                SetProperty(ref _startTime, value);
            }
        }

        public BookingModelState State
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

        public bool IsBookingEntryValid()
        {
            return !string.IsNullOrEmpty(Description)
                && Duration > TimeSpan.Zero;
        }

        private TimeSpan CalculateDurationBasedOnStartAndEndTime()
        {
            if (StartTime == null || EndTime == null)
            {
                return TimeSpan.Zero;
            }

            return EndTime.Value - StartTime.Value;
        }
    }
}