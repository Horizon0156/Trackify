using Horizon.Framework.Mvvm;

namespace BookingHelper.ViewModels
{
    using System.Windows.Input;

    public class Effort : ObserveableObject
    {
        private bool _markedAdBooked;

        private ICommandFactory _commandFactory;

        public Effort(ICommandFactory commandFactory, string description, double effortTimeInHours)
        {
            _commandFactory = commandFactory;
            MarkedAsBookedCommand = _commandFactory.CreateCommand(MarkedEffortAsBooked);
            EffortTimeInHours = effortTimeInHours;
            Description = description;
            MarkedAsBooked = false;
        }

        public ICommand MarkedAsBookedCommand { get; }

        private void MarkedEffortAsBooked()
        {
            MarkedAsBooked = !MarkedAsBooked;
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

            return new Effort(_commandFactory, Description, roundedEffort);
        }
    }
}