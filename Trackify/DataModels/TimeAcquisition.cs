using System;

namespace Trackify.DataModels
{
    internal class TimeAcquisition
    {
        public string Description { get; set; }

        public DateTime? StopTime { get; set; }

        public int Id { get; set; }

        public DateTime? StartTime { get; set; }

        public AcquisitionState State { get; set; }
    }
}