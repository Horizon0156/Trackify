using Horizon.Framework.Mvvm;

namespace BookingHelper.ViewModels
{
    using System.Windows.Input;

    public class Effort : ObserveableObject
    {
        private bool _markedAsBooked;

        private ICommandFactory _commandFactory;

        public Effort(ICommandFactory commandFactory, string description, double effortTimeInHours)
        {
            _commandFactory = commandFactory;
            MarkAsBookedCommand = _commandFactory.CreateCommand(MarkEffortAsBooked);
            EffortTimeInHours = effortTimeInHours;
            Description = description;
            MarkedAsBooked = false;
        }

        public ICommand MarkAsBookedCommand { get; }

        private void MarkEffortAsBooked()
        {
            MarkedAsBooked = !MarkedAsBooked;
        }

        public string Description { get; }

        public double EffortTimeInHours { get; private set; }

        public bool MarkedAsBooked
        {
            get
            {
                return _markedAsBooked;
            }
            set
            {
                SetProperty(ref _markedAsBooked, value);
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