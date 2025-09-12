using System;
using GameFramework.Network;

namespace GameMain.Runtime
{
    public sealed class PacketHandler : IPacketHandler
    {
        public static readonly int HandlerId = EventExtensions.GetEventId(typeof(PacketHandler));
        public int Id => HandlerId;

        public void Handle(object sender, Packet packet)
        {
            if (packet is not NetworkPacket networkPacket)
            {
                throw new ArgumentException("Packet is invalid.", nameof(packet));
            }

            if (sender is not INetworkChannel networkChannel)
            {
                throw new ArgumentException("Sender is invalid.", nameof(sender));
            }

            //TODO verify token
            if (NetworkMessageRouter.Instance.Dispatch(
                    networkChannel.Name,
                    networkPacket.Message.GetType(),
                    networkPacket.Message))
            {
                return;
            }

            GameEntry.Event.Fire(networkChannel, NetworkMessageEventArgs.Create(networkPacket.Message));
        }
    }
}
