using Horizon.MvvmFramework.Components;

namespace BookingHelper.ViewModels
{
    internal class BreakRegulation : ObserveableObject
    {
        private double _mandatoryBreakTime;
        private int _workEffortLimit;

        public BreakRegulation()
        {
        }

        public BreakRegulation(int workEffortLimit, double mandatoryBreakTime)
        {
            WorkEffortLimit = workEffortLimit;
            MandatoryBreakTime = mandatoryBreakTime;
        }

        public double MandatoryBreakTime
        {
            get
            {
                return _mandatoryBreakTime;
            }
            set
            {
                SetProperty(ref _mandatoryBreakTime, value);
            }
        }

        public int WorkEffortLimit
        {
            get
            {
                return _workEffortLimit;
            }
            set
            {
                SetProperty(ref _workEffortLimit, value);
            }
        }
    }
}