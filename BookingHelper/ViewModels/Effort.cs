using Horizon.Framework.Mvvm;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace BookingHelper.ViewModels
{
    internal class Effort : ObserveableObject
    {
        private readonly ICommandFactory _commandFactory;
        private bool _markedAdBooked;

        public Effort(ICommandFactory commandFactory, ICollection<BookingModel> desisiveBookings)
        {
            _commandFactory = commandFactory;
            DesisiveBookings = desisiveBookings;
            MarkedAsBookedCommand = _commandFactory.CreateCommand(MarkedEffortAsBooked);
            EffortTimeInHours = desisiveBookings.Sum(b => b.Duration.TotalHours);
            Description = desisiveBookings.First().Description;
            MarkedAsBooked = desisiveBookings.All(b => b.State == BookingModelState.Booked);
        }

        public ICollection<BookingModel> DesisiveBookings { get; set; }

        public string Description { get; }

        public double EffortTimeInHours { get; private set;  }

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

        public ICommand MarkedAsBookedCommand { get; }

        public Effort RoundEffort(double intervalTimeInHours)
        {
            var differenceToRound = EffortTimeInHours % intervalTimeInHours;
            var roundUp = differenceToRound >= intervalTimeInHours / 2;

            var roundedEffort = roundUp
                ? EffortTimeInHours - differenceToRound + intervalTimeInHours
                : EffortTimeInHours - differenceToRound;
            
            return new Effort(_commandFactory, DesisiveBookings) { EffortTimeInHours = roundedEffort };
        }

        private void MarkedEffortAsBooked()
        {
            MarkedAsBooked = !MarkedAsBooked;

            foreach (var booking in DesisiveBookings)
            {
                booking.State = MarkedAsBooked
                    ? BookingModelState.Booked
                    : BookingModelState.Recorded;
            }
        }
    }
}