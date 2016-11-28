using System;
using JetBrains.Annotations;

namespace BookingHelper.DataModels
{
    internal class Booking
    {
        public DateTime Date { get; [UsedImplicitly] set; }

        public string Description { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int Id { get; [UsedImplicitly] set; }

        public TimeSpan? StartTime { get; set; }

        public BookingState State { get; set; }
    }
}