namespace BookingHelper.ViewModels
{
    internal class Effort
    {
        public Effort(string description, double effortTimeInHours)
        {
            EffortTimeInHours = effortTimeInHours;
            Description = description;
        }

        public string Description { get; }

        public double EffortTimeInHours { get; private set; }

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
}