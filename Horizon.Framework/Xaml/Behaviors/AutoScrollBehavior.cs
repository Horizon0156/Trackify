using System;
using System.Windows;
using System.Windows.Controls;

using Horizon.Framework.Xaml.Interaction;

namespace Horizon.Framework.Xaml.Behaviors
{
    /// <summary>
    /// Behaviour which enables Auto scrolling for a scroll viewer.
    /// </summary>
    public class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        private double _latestRegisteredScrollViewerHeight;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.LayoutUpdated += ScrollToAddedRow;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.LayoutUpdated -= ScrollToAddedRow;
        }

        private void ScrollToAddedRow(object sender, EventArgs e)
        {
            if (_latestRegisteredScrollViewerHeight < AssociatedObject.ExtentHeight)
            {
                _latestRegisteredScrollViewerHeight = AssociatedObject.ExtentHeight;
                AssociatedObject.ScrollToEnd();
            }
        }
    }
}