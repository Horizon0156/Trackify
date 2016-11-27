namespace BookingHelper.Mocks
{
    internal interface ISettings
    {
        string AccentColor { get; set; }

        double BookingTimeInterval { get; set; }
    }
}