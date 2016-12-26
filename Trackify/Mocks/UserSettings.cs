using System;
using Horizon.MvvmFramework.Components;
using Trackify.Properties;

namespace Trackify.Mocks
{
    internal class UserSettings : ObserveableObject, ISettings
    {
        private const double TOLERANCE = 0.01;

        public string AccentColor
        {
            get
            {
                return Settings.Default.AccentColor;
            }
            set
            {
                if (Settings.Default.AccentColor != value)
                {
                    Settings.Default.AccentColor = value;
                    OnPropertyChanged();
                    Settings.Default.Save();
                }
            }
        }

        public double BookingTimeInterval
        {
            get
            {
                return Settings.Default.BookingTimeInterval;
            }
            set
            {
                if (Math.Abs(Settings.Default.BookingTimeInterval - value) > TOLERANCE)
                {
                    Settings.Default.BookingTimeInterval = value;
                    OnPropertyChanged();
                    Settings.Default.Save();
                }
            }
        }

        public double DailyTarget
        {
            get
            {
                return Settings.Default.DailyTarget;
            }
            set
            {
                if (Math.Abs(Settings.Default.DailyTarget - value) > TOLERANCE)
                {
                    Settings.Default.DailyTarget = value;
                    OnPropertyChanged();
                    Settings.Default.Save();
                }
            }
        }
    }
}