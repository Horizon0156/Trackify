using BookingHelper.Properties;

namespace BookingHelper.Mocks
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
    }
}