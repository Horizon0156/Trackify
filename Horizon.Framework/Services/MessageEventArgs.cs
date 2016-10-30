using System;
using Horizon.Framework.Exceptions;
using JetBrains.Annotations;

namespace Horizon.Framework.Services
{
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message"> The message. </param>
        /// <param name="title"> The title. </param>
        /// <param name="buttonSetup"> The button setup. </param>
        public MessageEventArgs([NotNull] string message, [CanBeNull] string title, MessageButtonSetup buttonSetup)
        {
            Throw.IfArgumentIsNull(message, nameof(message));

            ButtonSetup = buttonSetup;
            Message = message;
            Title = title;
            Result = MessageResult.None;
            MessageHandled = false;
        }

        /// <summary>
        /// Gets the button setup for the message.
        /// </summary>
        public MessageButtonSetup ButtonSetup { get; }

        /// <summary>
        /// Gets the Message.
        /// </summary>
        [NotNull]
        public string Message { get; }

        /// <summary>
        /// Gets or sets the result of the message.
        /// </summary>
        public MessageResult Result { get; set; }

        /// <summary>
        /// Gets the optional title of the message.
        /// </summary>
        [CanBeNull]
        public string Title { get; }

        /// <summary>
        /// Gets or sets a flag whether the message was already handled.
        /// </summary>
        public bool MessageHandled;
    }
}