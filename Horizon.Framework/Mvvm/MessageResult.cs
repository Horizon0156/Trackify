namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// Provides reults of a message box.
    /// </summary>
    public enum MessageResult
    {
        /// <summary>
        /// Used for indicating the closure of an informational message.
        /// </summary>
        Ok,

        /// <summary>
        /// Used for indicating agreement to the message.
        /// </summary>
        Yes,

        /// <summary>
        /// Used for indicating declinement to the message.
        /// </summary>
        No,

        /// <summary>
        /// Used for indicating cancellation of the message routine.
        /// </summary>
        Cancelled
    }
}