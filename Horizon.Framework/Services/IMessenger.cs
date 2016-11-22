using System;

namespace Horizon.Framework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessenger
    {
        void Send<T>(T message);

        void Register<T>(Action<T> messageHandler);

        void Unregister<T>();

        void Unregister(Type messageType);
    }
}