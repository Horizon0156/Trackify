using Trackify.Properties;

namespace Trackify.Mocks
{
    internal class UserSettings : ISettings
    {
        public string AccentColor
        {
            get
            {
                return Settings.Default.AccentColor;
            }
            set
            {
                Settings.Default.AccentColor = value;
                Settings.Default.Save();
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
                Settings.Default.BookingTimeInterval = value;
                Settings.Default.Save();
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
                Settings.Default.DailyTarget = value;
                Settings.Default.Save();
            }
        }
    }
}