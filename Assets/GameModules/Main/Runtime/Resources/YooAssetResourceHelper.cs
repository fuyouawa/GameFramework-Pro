using System;
using System.Linq;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;
using AssetInfo = GameFramework.Resource.AssetInfo;

namespace GameMain.Runtime
{
    /// <summary>
    /// 远端资源地址查询服务类
    /// </summary>
    class RemoteServices : IRemoteServices
    {
        private readonly string _defaultHostServer;
        private readonly string _fallbackHostServer;

        public RemoteServices(string defaultHostServer, string fallbackHostServer)
        {
            _defaultHostServer = defaultHostServer;
            _fallbackHostServer = fallbackHostServer;
        }

        string IRemoteServices.GetRemoteMainURL(string fileName)
        {
            return $"{_defaultHostServer}/{fileName}";
        }

        string IRemoteServices.GetRemoteFallbackURL(string fileName)
        {
            return $"{_fallbackHostServer}/{fileName}";
        }
    }

    public class YooAssetResourceHelper : ResourceHelperBase
    {
        private ResourceComponent _resourceComponent = null;

        private ResourceComponent ResourceComponent
        {
            get
            {
                if (_resourceComponent == null)
                {
                    _resourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
                    if (_resourceComponent == null)
                    {
                        throw new InvalidOperationException("Resource component is invalid.");
                    }
                }
                return _resourceComponent;
            }
        }

        public override bool CheckAssetNameValid(string packageName, string assetName)
        {
            var package = YooAssets.GetPackage(packageName);
            return package.CheckLocationValid(assetName);
        }

        public override bool IsNeedDownloadFromRemote(AssetInfo assetInfo)
        {
            var package = YooAssets.GetPackage(assetInfo.PackageName);
            return package.IsNeedDownloadFromRemote(assetInfo.UserData as YooAsset.AssetInfo);
        }

        public override AssetInfo GetAssetInfo(string packageName, string assetName)
        {
            var package = YooAssets.GetPackage(packageName);
            var assetInfo = package.GetAssetInfo(assetName);
            return new AssetInfo(assetInfo.PackageName, assetInfo.AssetType, assetName, assetInfo.Error, assetInfo);
        }

        public override AssetInfo[] GetAssetInfos(string packageName, string[] tags)
        {
            var package = YooAssets.GetPackage(packageName);
            var assetInfos = package.GetAssetInfos(tags);
            return assetInfos.Select(assetInfo => new AssetInfo(assetInfo.PackageName, assetInfo.AssetType,
                assetInfo.Address, assetInfo.Error, assetInfo)).ToArray();
        }

        public override void UnloadScene(string sceneAssetName, object sceneAsset,
            UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            var sceneHandle = sceneAsset as YooAsset.SceneHandle;
            var unloadOperation = sceneHandle.UnloadAsync();
            unloadOperation.Completed += op =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log.Debug($"Unload scene '{sceneAssetName}' succeed.");
                    unloadSceneCallbacks.UnloadSceneSuccessCallback?.Invoke(sceneAssetName, userData);
                }
                else
                {
                    Log.Error($"Unload scene '{sceneAssetName}' failure: {op.Error}.");
                    unloadSceneCallbacks.UnloadSceneFailureCallback?.Invoke(sceneAssetName, op.Error);
                }
            };
        }
    }
}
