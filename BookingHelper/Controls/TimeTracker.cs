using System;
using System.Windows;
using System.Windows.Controls;

namespace BookingHelper.Controls
{
    using System.Windows.Threading;

    using JetBrains.Annotations;

    internal class TimeTracker : TextBlock
    {
        [NotNull]
        private DispatcherTimer _updateTimer;

        private bool _intializeTimebase;

        public static DependencyProperty EllapsedTimeProperty = DependencyProperty.Register(
            "EllapsedTime",
            typeof(TimeSpan),
            typeof(TimeTracker),
            new FrameworkPropertyMetadata(TimeSpan.Zero));

        public static DependencyProperty IsRunningProperty = DependencyProperty.Register(
            "IsRunning",
            typeof(bool),
            typeof(TimeTracker),
            new FrameworkPropertyMetadata(defaultValue: false, propertyChangedCallback: StartOrStopTimeTracker));

        private DateTime _startTime;

        private TimeSpan _ellapsedTimeAtStart;

        public TimeTracker()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Tick += UpdateEllapsedTime;
            _updateTimer.Interval = TimeSpan.FromSeconds(1);
            
            EllapsedTime = TimeSpan.FromSeconds(50);
            _intializeTimebase = true;
            Text = "00:00:00";
        }

        private void UpdateEllapsedTime(object sender, EventArgs e)
        {
            EllapsedTime = _ellapsedTimeAtStart + (DateTime.Now - _startTime);

            if (EllapsedTime.TotalSeconds < 60)
            {
                Text = $"{(int) EllapsedTime.TotalSeconds} sec";
            }
            else if (EllapsedTime.TotalMinutes < 60)
            {
                Text = $"{EllapsedTime.ToString("mm\\:ss")} min";
            }
            else
            {
                Text = EllapsedTime.ToString("hh\\:mm\\:ss");
            }
        }

        public bool IsRunning
        {
            get
            {
                return (bool)GetValue(IsRunningProperty);
            }
            set
            {
                SetValue(IsRunningProperty, value);
            }
        }

        public TimeSpan EllapsedTime
        {
            get
            {
                return (TimeSpan) GetValue(EllapsedTimeProperty);
            }
            set
            {
                SetValue(EllapsedTimeProperty, value);
            }
        }

        private static void StartOrStopTimeTracker(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeTracker = (TimeTracker)d;

            timeTracker.StartOrStopTimeTracker();
        }

        private void StartOrStopTimeTracker()
        {
            if (IsRunning)
            {
                InitializeTimebase();
            }
            _updateTimer.IsEnabled = IsRunning;
        }

        private void InitializeTimebase()
        {
            _startTime = DateTime.Now;
            _ellapsedTimeAtStart = EllapsedTime;
        }
    }
}