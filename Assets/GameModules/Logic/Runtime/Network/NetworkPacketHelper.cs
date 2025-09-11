using System;
using GameMain.Runtime;

namespace GameLogic.Runtime
{
    public class NetworkPacketHelper : INetworkPacketHelper
    {
        public int GetMessageId(NetworkPacket packet)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySpan<byte> Serialize(NetworkPacket packet)
        {
            throw new NotImplementedException();
        }

        public NetworkPacket Deserialize(ReadOnlySpan<byte> data)
        {
            throw new NotImplementedException();
        }
    }
}
