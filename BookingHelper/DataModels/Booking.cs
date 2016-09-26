using System;

namespace BookingHelper.DataModels
{
    internal class Booking
    {
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public TimeSpan? EndTime { get; set; }

        public int Id { get; set; }

        public TimeSpan? StartTime { get; set; }
    }
}