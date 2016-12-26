namespace Trackify.Mocks
{
    internal interface ISettings
    {
        string AccentColor { get; set; }

        double BookingTimeInterval { get; set; }

        double DailyTarget { get; set; }
    }
}