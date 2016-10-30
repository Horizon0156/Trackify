using System;

namespace Horizon.Framework.Services
{
    /// <summary>
    /// Event based message service.
    /// The view should react to a message announcement.
    /// </summary>
    public sealed class MessageServiceEventManager : IMessageService
    {
        public event EventHandler<MessageEventArgs> MessageAnnouncement;

        /// <inheritdoc/>
        public MessageResult ShowMessage(string message)
        {
            return ShowMessage(message, null, MessageButtonSetup.Affirmitive);
        }

        /// <inheritdoc/>
        public MessageResult ShowMessage(string message, string title)
        {
            return ShowMessage(message, title, MessageButtonSetup.Affirmitive);
        }

        /// <inheritdoc/>
        public MessageResult ShowMessage(string message, string title, MessageButtonSetup buttonSetup)
        {
            var messageEvent = new MessageEventArgs(message, title, buttonSetup);
            OnMessageAnnouncement(messageEvent);

            return messageEvent.Result;
        }

        private void OnMessageAnnouncement(MessageEventArgs e)
        {
            MessageAnnouncement?.Invoke(this, e);
        }
    }
}