using System;
using System.Collections.Generic;

namespace GameMain.Runtime
{
    public class NetworkMessageRouter
    {
        private static readonly Lazy<NetworkMessageRouter> LazyInstance = new Lazy<NetworkMessageRouter>();
        public static NetworkMessageRouter Instance => LazyInstance.Value;

        private readonly Dictionary<Type, Delegate> _messageHandlersByMessageType = new Dictionary<Type, Delegate>();

        public void RegisterHandler<T>(NetworkMessageHandler<T> handler)
        {
            RegisterHandler(typeof(T), handler);
        }

        public bool Dispatch<T>(string channelName, T message)
        {
            return Dispatch(channelName, typeof(T), message);
        }

        public void RegisterHandler(Type messageType, Delegate handler)
        {
            if (_messageHandlersByMessageType.TryGetValue(messageType, out var existingHandler))
            {
                existingHandler = Delegate.Combine(existingHandler, handler);
                _messageHandlersByMessageType[messageType] = existingHandler;
            }
            else
            {
                _messageHandlersByMessageType.Add(messageType, handler);
            }
        }

        public bool Dispatch(string channelName, Type messageType, object message)
        {
            if (_messageHandlersByMessageType.TryGetValue(messageType, out var handler))
            {
                handler.DynamicInvoke(channelName, message);
                return true;
            }

            return false;
        }
    }
}
