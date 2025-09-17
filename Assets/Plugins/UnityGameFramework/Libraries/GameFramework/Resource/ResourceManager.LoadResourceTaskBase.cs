using System;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private abstract class LoadResourceTaskBase : TaskBase
        {
            private static int s_Serial = 0;

            private string m_PackageName;
            private string m_AssetName;
            private Type m_AssetType;

            public string PackageName => m_PackageName;
            public string AssetName => m_AssetName;
            public Type AssetType => m_AssetType;

            public DateTime StartTime { get; set; }

            public abstract bool IsScene { get; }


            public virtual void OnLoadAssetSuccess(LoadResourceAgent agent, AssetObject assetObject, float duration)
            {
            }

            public virtual void OnLoadAssetFailure(LoadResourceAgent agent, LoadResourceStatus status, string errorMessage)
            {
            }

            protected void Initialize(string packageName, string assetName, Type assetType, int priority, object userData)
            {
                Initialize(++s_Serial, "LoadResourceTask", priority, userData);
                m_PackageName = packageName;
                m_AssetName = assetName;
                m_AssetType = assetType;
            }
        }
    }
}
