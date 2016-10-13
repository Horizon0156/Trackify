namespace BookingHelper.ViewModels
{
    internal class BreakRegulation
    {
        internal BreakRegulation(int workEffortLimit, double mandatoryBreakTime)
        {
            WorkEffortLimit = workEffortLimit;
            MandatoryBreakTime = mandatoryBreakTime;
        }

        public double MandatoryBreakTime { get; }

        public int WorkEffortLimit { get; }
    }
}