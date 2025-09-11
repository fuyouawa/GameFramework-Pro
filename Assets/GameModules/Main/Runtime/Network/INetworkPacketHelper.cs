using System;

namespace GameMain.Runtime
{
    public interface INetworkPacketHelper
    {
        int GetMessageId(NetworkPacket packet);
        ReadOnlySpan<byte> Serialize(NetworkPacket packet);
        NetworkPacket Deserialize(ReadOnlySpan<byte> data);
    }
}
