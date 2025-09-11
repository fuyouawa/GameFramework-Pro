using GameFramework.Network;

namespace GameMain.Runtime
{
    public class PacketHeader : IPacketHeader
    {
        public int PacketLength { get; }

        public PacketHeader(int packetLength)
        {
            PacketLength = packetLength;
        }
    }
}
