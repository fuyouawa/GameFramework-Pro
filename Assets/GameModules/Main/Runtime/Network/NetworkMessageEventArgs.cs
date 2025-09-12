using GameFramework;

namespace GameMain.Runtime
{
    public class NetworkMessageEventArgs : GameEventArgs
    {
        public object Message { get; private set; }

        public static NetworkMessageEventArgs Create(object message)
        {
            var eventArgs = ReferencePool.Acquire<NetworkMessageEventArgs>();
            eventArgs.Message = message;
            return eventArgs;
        }

        public override void Clear()
        {
            Message = null;
        }
    }
}
