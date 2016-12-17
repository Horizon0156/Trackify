using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Horizon.MvvmFramework.Commands;
using Horizon.MvvmFramework.Components;

namespace BookingHelper.ViewModels
{
    internal class Effort : ObserveableObject
    {
        private readonly ICommandFactory _commandFactory;
        private bool _markedAsBooked;

        public Effort(ICommandFactory commandFactory, ICollection<TimeAcquisitionModel> desisiveBookings)
        {
            _commandFactory = commandFactory;
            DesisiveBookings = desisiveBookings;
            MarkAsBookedCommand = _commandFactory.CreateCommand(MarkedEffortAsBooked);
            EffortTimeInHours = desisiveBookings.Sum(b => b.Duration?.TotalHours ?? 0);
            Description = desisiveBookings.First().Description;
            MarkedAsBooked = desisiveBookings.All(b => b.State == TimeAcquisitionStateModel.Booked);
        }

        public ICollection<TimeAcquisitionModel> DesisiveBookings { get; set; }

        public string Description { get; }

        public double EffortTimeInHours { get; private set;  }

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

        public ICommand MarkAsBookedCommand { get; }

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
                    ? TimeAcquisitionStateModel.Booked
                    : TimeAcquisitionStateModel.Recorded;
            }
        }
    }
}