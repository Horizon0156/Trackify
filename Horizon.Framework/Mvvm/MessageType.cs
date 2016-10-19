namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// Enumeration for the type of a message.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// The message is an information.
        /// </summary>
        Information, 

        /// <summary>
        /// The message is a question.
        /// </summary>
        Question,

        /// <summary>
        /// The message is an important question.
        /// </summary>
        ImportantQuestion,

        /// <summary>
        /// The message is a warning.
        /// </summary>
        Warning,

        /// <summary>
        /// The message indicates an error.
        /// </summary>
        Error
    }
}
