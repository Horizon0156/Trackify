using Horizon.Framework.Mvvm;
using System;

namespace BookingHelper.ViewModels
{
    internal class BookingModel : ViewModel
    {
        private string _description;

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

        public TimeSpan? EndTime { get; set; }

        public int Id { get; set; }

        public TimeSpan? StartTime { get; set; }

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