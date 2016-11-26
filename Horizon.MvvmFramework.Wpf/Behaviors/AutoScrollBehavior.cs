using System;
using System.Windows.Controls;
using System.Windows.Interactivity;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Wpf.Behaviors
{
    /// <summary>
    /// Behaviour which enables Auto scrolling for a scroll viewer.
    /// </summary>
    public sealed class AutoScrollBehavior : Behavior<ScrollViewer>
    {
        private double _latestRegisteredScrollViewerHeight;

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.LayoutUpdated += ScrollToAddedRow;
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.LayoutUpdated -= ScrollToAddedRow;
        }

        private void ScrollToAddedRow([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (_latestRegisteredScrollViewerHeight < AssociatedObject.ExtentHeight)
            {
                _latestRegisteredScrollViewerHeight = AssociatedObject.ExtentHeight;
                AssociatedObject.ScrollToEnd();
            }
        }
    }
}