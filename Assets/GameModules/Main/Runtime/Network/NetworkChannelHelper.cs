using System;
using System.IO;
using System.Net;
using GameFramework.Network;

namespace GameMain.Runtime
{
    public class NetworkChannelHelper : INetworkChannelHelper
    {
        private static INetworkChannelAgent s_agent;

        public int PacketHeaderLength { get; private set; }
        private readonly byte[] _cache = new byte[1024];

        public static void SetAgent(INetworkChannelAgent agent)
        {
            s_agent = agent;
        }

        public void Initialize(INetworkChannel networkChannel)
        {
            networkChannel.RegisterHandler(new PacketHandler());

            var info = s_agent.GetConnectionInfo(networkChannel.Name);
            PacketHeaderLength = info.PacketHeaderLength;
            networkChannel.Connect(IPAddress.Parse(info.IP), info.Port);
        }

        public void Shutdown()
        {
        }

        public void PrepareForConnecting()
        {
        }

        public bool SendHeartBeat()
        {
            return true;
        }

        public bool Serialize<T>(T packet, Stream destination) where T : Packet
        {
            if (packet is not NetworkPacket networkPacket)
            {
                throw new ArgumentException($"Packet '{packet}' is invalid.", nameof(packet));
            }
            destination.Write(networkPacket.Serialize());
            return true;
        }

        public IPacketHeader DeserializePacketHeader(Stream source, out object customErrorData)
        {
            customErrorData = null;
            var i = source.Read(_cache, 0, PacketHeaderLength);
            if (i != PacketHeaderLength)
            {
                return null;
            }
            var packetLength = BitConverter.ToInt32(_cache, 0);
            return new PacketHeader(IPAddress.NetworkToHostOrder(packetLength));
        }

        public Packet DeserializePacket(IPacketHeader packetHeader, Stream source, out object customErrorData)
        {
            customErrorData = null;
            var i = source.Read(_cache, 0, packetHeader.PacketLength);
            if (i != packetHeader.PacketLength)
            {
                return null;
            }
            return NetworkPacket.Deserialize(new ReadOnlySpan<byte>(_cache, 0, packetHeader.PacketLength));
        }
    }
}
