using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework.FileSystem;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain
{
    public sealed class YooAssetLoadResourceAgentHelper : LoadResourceAgentHelperBase
    {
        public override event EventHandler<LoadResourceAgentHelperLoadCompleteEventArgs> LoadComplete;

        public override event EventHandler<LoadResourceAgentHelperErrorEventArgs> Error;

        public override void LoadAsset(string packageName, string assetName, Type assetType, bool isScene)
        {
            var package = YooAssets.GetPackage(packageName);
            var assetHandle = assetType == null
                ? package.LoadAssetAsync(assetName)
                : package.LoadAssetAsync(assetName, assetType);

            assetHandle.Completed += op =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    LoadComplete?.Invoke(this, LoadResourceAgentHelperLoadCompleteEventArgs.Create(op.AssetObject));
                }
                else
                {
                    Error?.Invoke(this, LoadResourceAgentHelperErrorEventArgs.Create(LoadResourceStatus.AssetError, op.LastError));
                }
            };
        }

        public override void Reset()
        {
        }
    }
}
