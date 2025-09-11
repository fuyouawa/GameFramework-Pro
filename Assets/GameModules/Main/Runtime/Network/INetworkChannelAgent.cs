namespace GameMain.Runtime
{
    public struct NetworkChannelConnectionInfo
    {
        public int PacketHeaderLength;
        public string IP;
        public int Port;
    }

    public interface INetworkChannelAgent
    {
        NetworkChannelConnectionInfo GetConnectionInfo(string channelName);
    }
}
