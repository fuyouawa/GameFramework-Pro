namespace GameMain.Runtime
{
    public static class INetworkServiceExtensions
    {
        public static void RegisterMessageHandler<T>(this INetworkService networkService,
            NetworkMessageHandler<T> handler)
        {
            NetworkMessageRouter.Instance.RegisterHandler(typeof(T), handler);
        }
    }
}
