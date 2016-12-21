﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Trackify.Controls
{
    internal class TimeTracker : TextBlock
    {
        public static readonly DependencyProperty IsRunningProperty = DependencyProperty.Register(
            "IsRunning",
            typeof(bool),
            typeof(TimeTracker),
            new FrameworkPropertyMetadata(defaultValue: false, propertyChangedCallback: ToggleTimeTracking));

        public static readonly DependencyProperty StartTimeProperty = DependencyProperty.Register(
            "StartTime",
            typeof(DateTime?),
            typeof(TimeTracker),
            new FrameworkPropertyMetadata(defaultValue: null, propertyChangedCallback: UpdateEllapsedTime));

        public static readonly DependencyProperty StopTimeProperty = DependencyProperty.Register(
           "StopTime",
           typeof(DateTime?),
           typeof(TimeTracker),
           new FrameworkPropertyMetadata(defaultValue: null, propertyChangedCallback: UpdateEllapsedTime));

        private readonly DispatcherTimer _updateTimer;

        public TimeTracker()
        {
            _updateTimer = new DispatcherTimer();
            _updateTimer.Tick += UpdateEllapsedTime;
            _updateTimer.Interval = TimeSpan.FromSeconds(1);

            Text = "00:00:00";
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

        public DateTime? StartTime
        {
            get
            {
                return (DateTime?)GetValue(StartTimeProperty);
            }
            set
            {
                SetValue(StartTimeProperty, value);
            }
        }

        public DateTime? StopTime
        {
            get
            {
                return (DateTime?)GetValue(StopTimeProperty);
            }
            set
            {
                SetValue(StopTimeProperty, value);
            }
        }

        private static void ToggleTimeTracking(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeTracker = (TimeTracker)d;

            timeTracker.ToggleTimeTracking();
        }

        private static void UpdateEllapsedTime(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var timeTracker = (TimeTracker)d;
            timeTracker.UpdateEllapsedTime((timeTracker.StopTime - timeTracker.StartTime) ?? TimeSpan.Zero);
        }

        private void ToggleTimeTracking()
        {
            _updateTimer.IsEnabled = IsRunning;

            if (IsRunning)
            {
                StartTime = !StartTime.HasValue || StopTime.HasValue
                    ? DateTime.Now
                    : StartTime;
                StopTime = null;

                UpdateEllapsedTime(DateTime.Now - StartTime.Value);
            }
            else
            {
                StopTime = DateTime.Now;
                UpdateEllapsedTime((StopTime - StartTime) ?? TimeSpan.Zero);
            }
        }

        private void UpdateEllapsedTime(object sender, EventArgs e)
        {
            UpdateEllapsedTime((DateTime.Now - StartTime) ?? TimeSpan.Zero);
        }

        private void UpdateEllapsedTime(TimeSpan ellapsedTime)
        {
            if (ellapsedTime.TotalSeconds < 1)
            {
                Text = "00:00:00";
            }
            else if (ellapsedTime.TotalSeconds < 60)
            {
                Text = $"{(int)ellapsedTime.TotalSeconds} sec";
            }
            else if (ellapsedTime.TotalMinutes < 60)
            {
                Text = $"{ellapsedTime:m\\:ss} min";
            }
            else
            {
                Text = ellapsedTime.ToString("hh\\:mm\\:ss");
            }
        }
    }
}