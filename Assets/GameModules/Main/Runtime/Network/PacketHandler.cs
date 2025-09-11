using GameFramework.Network;

namespace GameMain.Runtime
{
    public sealed class PacketHandler : IPacketHandler
    {
        public static readonly int HandlerId = EventExtensions.GetEventId(typeof(PacketHandler));
        public int Id => HandlerId;

        public void Handle(object sender, Packet packet)
        {

        }
    }
}
