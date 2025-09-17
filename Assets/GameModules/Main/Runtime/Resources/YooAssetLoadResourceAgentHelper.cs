using System;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain.Runtime
{
    public sealed class YooAssetLoadResourceAgentHelper : LoadResourceAgentHelperBase
    {
        public override event EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> LoadComplete;

        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> Error;

        public override void LoadAsset(string packageName, string assetName, Type assetType, bool isScene)
        {
            var package = YooAssetsHelper.GetPackage(packageName);

            if (isScene)
            {
                
            }
            else
            {
                var assetHandle = assetType == null
                    ? package.LoadAssetAsync(assetName)
                    : package.LoadAssetAsync(assetName, assetType);

                assetHandle.Completed += handle =>
                {
                    if (handle.Status == EOperationStatus.Succeed)
                    {
                        LoadComplete?.Invoke(this, LoadResourceAgentHelperLoadCompleteEventArgs.Create(AssetObject.Create(handle.AssetObject, false, handle)));
                    }
                    else
                    {
                        Error?.Invoke(this, LoadResourceAgentHelperErrorEventArgs.Create(LoadResourceStatus.AssetError, handle.LastError));
                    }
                };
            }
        }

        public override void Reset()
        {
        }
    }
}
