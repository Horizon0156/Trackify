using System.ComponentModel;

namespace Trackify.Mocks
{
    internal interface ISettings : INotifyPropertyChanged
    {
        string AccentColor { get; set; }

        double BookingTimeInterval { get; set; }

        double DailyTarget { get; set; }

        bool IsDailyReportVisible { get; set; }

        bool ShouldApplicationStayAlwaysOnTop { get; set; }

        bool ShouldApplicationStartWithWindows { get; set; }
    }
}