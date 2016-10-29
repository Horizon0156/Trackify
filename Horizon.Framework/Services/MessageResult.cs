namespace Horizon.Framework.Services
{
    /// <summary>
    /// Enumeration for message results.
    /// </summary>
    public enum MessageResult
    {
        /// <summary>
        /// The message does not have a result.
        /// </summary>
        None, 

        /// <summary>
        /// The message was confimed.
        /// </summary>
        Affirmitive,

        /// <summary>
        /// The message was answered negative.
        /// </summary>
        Negative
    }
}