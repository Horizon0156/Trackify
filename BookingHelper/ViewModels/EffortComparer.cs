using System;
using System.Collections.Generic;

namespace BookingHelper.ViewModels
{
    internal class EffortComparer : IEqualityComparer<Effort>
    {
        private const double TIMESPAN_TOLERANCE = 0.01;

        public bool Equals(Effort x, Effort y)
        {
            if (x == null && y == null) { return true; }
            if (x == null || y == null) { return false; }

            return x.Description == y.Description && Math.Abs(x.EffortTimeInHours - y.EffortTimeInHours) < TIMESPAN_TOLERANCE;
        }

        public int GetHashCode(Effort obj)
        {
            return obj.GetHashCode();
        }
    }
}