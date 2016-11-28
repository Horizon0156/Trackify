using System;

namespace Horizon.MvvmFramework.Services
{
    /// <summary>
    /// A messenger service used to send messages to other components.
    /// </summary>
    public interface IMessenger
    {
        /// <summary>
        /// Sends the given message instance.
        /// </summary>
        /// <typeparam name="T"> The type of the message. </typeparam>
        /// <param name="message"> The message. </param>
        void Send<T>(T message);

        /// <summary>
        /// Regsiters an action which will be executed if a message of the
        /// given type is received.
        /// </summary>
        /// <typeparam name="T"> The type of the message which should be received. </typeparam>
        /// <param name="messageHandler"> The message handler. </param>
        void Register<T>(Action<T> messageHandler);

        /// <summary>
        /// Unregsiters a message handler of the given type.
        /// </summary>
        /// <typeparam name="T"> The type of the message which should not be handled anymore. </typeparam>
        void Unregister<T>();

        /// <summary>
        /// Unregsiters a message handler of the given type.
        /// </summary>
        /// <param name="messageType"> The type of the message which should not be handled anymore.  </param>
        void Unregister(Type messageType);
    }
}