using System;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private sealed class LoadAssetTask : LoadResourceTaskBase
        {
            private LoadAssetCallbacks m_LoadAssetCallbacks;

            public override bool IsScene => false;

            public override void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject assetObject, float duration)
            {
                if (m_LoadAssetCallbacks.LoadAssetSuccessCallback != null)
                {
                    m_LoadAssetCallbacks.LoadAssetSuccessCallback(PackageName, AssetName, assetObject.Asset, duration, UserData);
                }
            }

            public override void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
            {
                if (m_LoadAssetCallbacks.LoadAssetFailureCallback != null)
                {
                    m_LoadAssetCallbacks.LoadAssetFailureCallback(PackageName, AssetName, status, errorMessage, UserData);
                }
            }

            public static LoadAssetTask Create(string packageName, string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks,
                object userData)
            {
                LoadAssetTask loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
                loadAssetTask.Initialize(packageName, assetName, assetType, priority, userData);
                loadAssetTask.m_LoadAssetCallbacks = loadAssetCallbacks;
                return loadAssetTask;
            }

            public override void Clear()
            {
                base.Clear();
                m_LoadAssetCallbacks = null;
            }
        }
    }
}
