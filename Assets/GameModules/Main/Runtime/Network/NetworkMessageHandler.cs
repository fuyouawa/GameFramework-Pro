namespace GameMain.Runtime
{
    public delegate void NetworkMessageHandler<T>(string networkChannelName, T message);
}
