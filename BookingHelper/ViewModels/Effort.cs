using Horizon.Framework.Mvvm;
using System.Collections.Generic;

namespace BookingHelper.ViewModels
{
    public class Effort : ViewModel
    {
        private bool _markedAdBooked;

        public Effort(string description, double effortTimeInHours)
        {
            EffortTimeInHours = effortTimeInHours;
            Description = description;
            MarkedAsBooked = false;
        }        

        public string Description { get; }

        public double EffortTimeInHours { get; private set; }

        public bool MarkedAsBooked
        {
            get
            {
                return _markedAdBooked;
            }
            set
            {
                SetProperty(ref _markedAdBooked, value);
            }
        }

        public Effort RoundEffort(double intervalTimeInHours)
        {
            var differenceToRound = EffortTimeInHours % intervalTimeInHours;

            var roundUp = differenceToRound >= intervalTimeInHours / 2;

            var roundedEffort = roundUp
                ? EffortTimeInHours - differenceToRound + intervalTimeInHours
                : EffortTimeInHours - differenceToRound;

            return new Effort(Description, roundedEffort);
        }
    }

    public class EffortComparer : IEqualityComparer<Effort>
    {
        public bool Equals(Effort x, Effort y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }

            return x.Description == y.Description && x.EffortTimeInHours == y.EffortTimeInHours;
        }

        public int GetHashCode(Effort obj)
        {
            return obj.GetHashCode();
        }
    }
}