
using System;
using JetBrains.Annotations;

namespace Horizon.Framework.Services
{
    /// <summary>
    /// Interface for a message service used to fire message boxes in a view independent way.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>
        /// Event for a message announcement.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageAnnouncement;

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="message"> The message which has to be shown. </param>
        /// <returns> A result. </returns>
        MessageResult ShowMessage([NotNull] string message);

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="title"> The title of the message box. </param>
        /// <param name="message"> The message which has to be shown. </param>
        /// <returns> A result. </returns>
        MessageResult ShowMessage([NotNull] string message, [CanBeNull] string title);

        /// <summary>
        /// Shows a message box.
        /// </summary>
        /// <param name="title"> The title of the message box. </param>
        /// <param name="message"> The message which has to be shown. </param>
        /// <param name="buttonSetup"> The button setup for the box. </param>
        /// <returns> A result. </returns>
        MessageResult ShowMessage([NotNull] string message, [CanBeNull] string title, MessageButtonSetup buttonSetup);
    }
}