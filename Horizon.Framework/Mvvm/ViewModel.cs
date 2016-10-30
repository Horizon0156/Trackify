using System;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// This class provides the base functionality for a ViewModel.
    /// </summary>
    public abstract class ViewModel : ObserveableObject
    {
        /// <summary>
        /// Event indicating that this dialog request a closure of itself.
        /// </summary>
        public event EventHandler ClosureRequested;

        /// <summary>
        /// Notifies a closure request.
        /// </summary>
        protected void OnClosureRequested()
        {
            ClosureRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}