using System;
using System.Collections.Generic;

namespace Horizon.Framework.Services
{
    public class MessageHub : IMessenger
    {
        private readonly Dictionary<Type, object> _messageHandlerByType;

        public MessageHub()
        {
            _messageHandlerByType = new Dictionary<Type, object>();
        }

        public void Register<T>(Action<T> messageHandler)
        {
            if (_messageHandlerByType.ContainsKey(typeof(T)))
            {
                Unregister(typeof(T));
            }
            _messageHandlerByType.Add(typeof(T), messageHandler);
        }

        public void Send<T>(T message)
        {
            if (_messageHandlerByType.ContainsKey(typeof(T)))
            {
                var messageHandler = _messageHandlerByType[typeof(T)] as Action<T>;
                messageHandler?.Invoke(message);
            }
        }

        public void Unregister<T>()
        {
            Unregister(typeof(T));
        }

        public void Unregister(Type messageType)
        {
            _messageHandlerByType.Remove(messageType);
        }
    }
}