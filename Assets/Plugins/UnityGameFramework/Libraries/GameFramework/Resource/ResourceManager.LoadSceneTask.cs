namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private class LoadSceneTask : LoadResourceTaskBase
        {
            private LoadSceneCallbacks m_LoadSceneCallbacks;

            public override bool IsScene => true;

            public static LoadSceneTask Create(string packageName, string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData)
            {
                LoadSceneTask loadSceneTask = ReferencePool.Acquire<LoadSceneTask>();
                loadSceneTask.Initialize(packageName, sceneAssetName, null, priority, userData);
                loadSceneTask.m_LoadSceneCallbacks = loadSceneCallbacks;
                return loadSceneTask;
            }

            public override void Clear()
            {
                base.Clear();
                m_LoadSceneCallbacks = null;
            }

            public override void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject assetObject, float duration)
            {
                if (m_LoadSceneCallbacks.LoadSceneSuccessCallback != null)
                {
                    m_LoadSceneCallbacks.LoadSceneSuccessCallback(PackageName, AssetName, assetObject.Asset, duration, UserData);
                }
            }

            public override void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
            {
                if (m_LoadSceneCallbacks.LoadSceneFailureCallback != null)
                {
                    m_LoadSceneCallbacks.LoadSceneFailureCallback(PackageName, AssetName, status, errorMessage, UserData);
                }
            }
        }
    }
}
