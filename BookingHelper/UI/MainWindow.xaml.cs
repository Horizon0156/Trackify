﻿using BookingHelper.ViewModels;
using System;
using System.Windows.Controls;

namespace BookingHelper.UI
{
    internal partial class MainWindow
    {
        public MainWindow(MainWindowViewModel dataContext)
        {
            InitializeComponent();

            DataContext = dataContext;
        }

        private void PrefillWithCurrentTimeIfEmpty(object sender, System.Windows.RoutedEventArgs e)
        {
            var textbox = (TextBox)sender;

            if (string.IsNullOrEmpty(textbox.Text))
            {
                textbox.Text = DateTime.Now.TimeOfDay.ToString(@"hh\:mm");
            }
        }
    }
}