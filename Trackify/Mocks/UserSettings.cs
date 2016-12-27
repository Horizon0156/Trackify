using System;
using System.Diagnostics;
using System.Reflection;
using Horizon.MvvmFramework.Components;
using Microsoft.Win32;
using Trackify.Properties;

namespace Trackify.Mocks
{
    internal class UserSettings : ObserveableObject, ISettings
    {
        private const double TOLERANCE = 0.005;
        private const string AUTOSTART_REGISTRY_KEY = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        private const string APPLICATION_NAME = "Trackify";

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

        public bool IsDailyReportVisible
        {
            get
            {
                return Settings.Default.IsDailyReportVisible;
            }
            set
            {
                if (Settings.Default.IsDailyReportVisible != value)
                {
                    Settings.Default.IsDailyReportVisible = value;
                    OnPropertyChanged();
                    Settings.Default.Save();
                }
            }
        }

        public bool ShouldApplicationStayAlwaysOnTop
        {
            get
            {
                return Settings.Default.ShouldApplicationStayAlwaysOnTop;
            }
            set
            {
                if (Settings.Default.ShouldApplicationStayAlwaysOnTop != value)
                {
                    Settings.Default.ShouldApplicationStayAlwaysOnTop = value;
                    OnPropertyChanged();
                    Settings.Default.Save();
                }
            }
        }

        public bool ShouldApplicationStartWithWindows
        {
            get
            {
                return IsAutostartEnabled();
            }
            set
            {
                if (value)
                {
                    EnableAutostart();
                }
                else
                {
                    DisableAutostart();
                }
            }
        }

        private RegistryKey GetAutostartRegistryKey(bool openWriteable)
        {
            return Registry.CurrentUser.OpenSubKey(AUTOSTART_REGISTRY_KEY, openWriteable);
        }

        private bool IsAutostartEnabled()
        {
            using (var key = GetAutostartRegistryKey(openWriteable: false))
            {
                return key.GetValue(APPLICATION_NAME) != null;
            }
        }

        private void EnableAutostart()
        {
            using (var key = GetAutostartRegistryKey(openWriteable: true))
            {
                var applicationPath = Assembly.GetExecutingAssembly().Location;
                Debug.Assert(applicationPath != null);

                key.SetValue(APPLICATION_NAME, applicationPath);
            }
        }

        private void DisableAutostart()
        {
            using (var key = GetAutostartRegistryKey(openWriteable: true))
            {
                key.DeleteValue(APPLICATION_NAME, throwOnMissingValue: false);
            }
        }
    }
}