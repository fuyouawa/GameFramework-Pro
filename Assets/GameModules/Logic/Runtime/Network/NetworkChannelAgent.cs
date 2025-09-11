using System;
using GameMain.Runtime;
using GameProto.Runtime.Config.network;

namespace GameLogic.Runtime
{
    public class NetworkChannelAgent : INetworkChannelAgent
    {
        public NetworkChannelConnectionInfo GetConnectionInfo(string channelName)
        {
            var servers = GameEntry.DataTable.GetDataTable<Server>().GetAllDataRows();
            foreach (var server in servers)
            {
                if (server.Type.ToString() == channelName)
                {
                    return new NetworkChannelConnectionInfo()
                    {
                        IP = server.Host,
                        Port = server.Port,
                        PacketHeaderLength = server.PackageHeaderLength
                    };
                }
            }
            throw new InvalidOperationException($"Invalid server type: {channelName}.");
        }
    }
}
