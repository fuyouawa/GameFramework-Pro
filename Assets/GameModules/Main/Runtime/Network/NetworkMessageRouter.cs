using System;
using System.Collections.Generic;
using EasyToolKit.Core;

namespace GameMain.Runtime
{
    public class NetworkMessageRouter : Singleton<NetworkMessageRouter>
    {
        private readonly Dictionary<Type, Delegate> _messageHandlersByMessageType = new Dictionary<Type, Delegate>();

        private NetworkMessageRouter() {}

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
