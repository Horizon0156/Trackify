namespace BookingHelper.DataModels
{
    internal class BreakRegulation
    {
        public int WorkEffortLimit { get; set; }
        public double MandatoryBreakTime { get; set; }

        internal BreakRegulation(int workEffortLimit, double mandatoryBreakTime)
        {
            WorkEffortLimit = workEffortLimit;
            MandatoryBreakTime = mandatoryBreakTime;
        }
    }
}
