namespace BookingHelper.DataModels
{
    internal class BreakRegulation
    {
        internal BreakRegulation(int workEffortLimit, double mandatoryBreakTime)
        {
            WorkEffortLimit = workEffortLimit;
            MandatoryBreakTime = mandatoryBreakTime;
        }

        public double MandatoryBreakTime { get; set; }

        public int WorkEffortLimit { get; set; }
    }
}