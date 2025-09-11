using System;
using GameFramework.Network;
using Google.Protobuf;

namespace GameMain.Runtime
{
    public sealed class NetworkPacket : Packet
    {
        private static INetworkPacketHelper s_helper;

        private int _id;
        private long _token;
        private object _message;

        public NetworkPacket(long token, object message)
        {
            _token = token;
            _message = message;
            _id = s_helper.GetMessageId(this);
        }

        public override int Id => _id;
        public long Token => _token;
        public object Message => _message;

        public static void SetHelper(INetworkPacketHelper helper)
        {
            s_helper = helper;
        }

        public ReadOnlySpan<byte> Serialize()
        {
            return s_helper.Serialize(this);
        }

        public static NetworkPacket Deserialize(ReadOnlySpan<byte> data)
        {
            return s_helper.Deserialize(data);
        }

        public override void Clear()
        {
            _id = 0;
            _token = 0;
            _message = null;
        }
    }
}
