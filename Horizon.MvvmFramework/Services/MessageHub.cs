using System;
using System.Collections.Generic;

namespace Horizon.MvvmFramework.Services
{
    /// <summary>
    /// Messagin Hub which provides functionality for an instance of <see cref="IMessenger"/>
    /// </summary>
    public class MessageHub : IMessenger
    {
        private readonly Dictionary<Type, object> _messageHandlerByType;

        /// <summary>
        /// Creates a new message hub.
        /// </summary>
        public MessageHub()
        {
            _messageHandlerByType = new Dictionary<Type, object>();
        }

        /// <inheritdoc/>
        public void Register<T>(Action<T> messageHandler)
        {
            if (_messageHandlerByType.ContainsKey(typeof(T)))
            {
                Unregister(typeof(T));
            }
            _messageHandlerByType.Add(typeof(T), messageHandler);
        }

        /// <inheritdoc/>
        public void Send<T>(T message)
        {
            if (_messageHandlerByType.ContainsKey(typeof(T)))
            {
                var messageHandler = _messageHandlerByType[typeof(T)] as Action<T>;
                messageHandler?.Invoke(message);
            }
        }

        /// <inheritdoc/>
        public void Unregister<T>()
        {
            Unregister(typeof(T));
        }

        /// <inheritdoc/>
        public void Unregister(Type messageType)
        {
            _messageHandlerByType.Remove(messageType);
        }
    }
}