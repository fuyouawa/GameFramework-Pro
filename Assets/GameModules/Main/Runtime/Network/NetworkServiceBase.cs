using UnityEngine;

namespace GameMain.Runtime
{
    public abstract class NetworkServiceBase : MonoBehaviour, INetworkService
    {
        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            RegisterMessageHandlers();
        }

        protected abstract void RegisterMessageHandlers();
    }
}
