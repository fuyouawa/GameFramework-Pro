using System;
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

        private void Start()
        {
            _resourceComponent = UnityGameFramework.Runtime.GameEntry.GetComponent<ResourceComponent>();
            if (_resourceComponent == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }
        }

        public override void Initialize()
        {
            // 初始化资源系统
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new ResourceLogger());
            }
            YooAssets.SetOperationSystemMaxTimeSlice(_resourceComponent.Milliseconds);

            // 创建默认的资源包
            string packageName = _resourceComponent.DefaultPackageName;
            var defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }
        }

        public override void InitPackage(string packageName, InitPackageCallbacks initPackageCallbacks)
        {
            // 创建资源包裹类
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                package = YooAssets.CreatePackage(packageName);
            }

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            if (_resourceComponent.PlayMode == PlayMode.EditorSimulateMode)
            {
                var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                var packageRoot = buildResult.PackageRootDirectory;
                var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                var initParameters = new EditorSimulateModeParameters();
                initParameters.EditorFileSystemParameters = editorFileSystemParams;
                initializationOperation = package.InitializeAsync(initParameters);
            }
            // 单机运行模式
            else if (_resourceComponent.PlayMode == PlayMode.OfflinePlayMode)
            {
                var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                var initParameters = new OfflinePlayModeParameters();
                initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                initializationOperation = package.InitializeAsync(initParameters);
            }
            // 联机运行模式
            else if (_resourceComponent.PlayMode == PlayMode.HostPlayMode)
            {
                IRemoteServices remoteServices = new RemoteServices(_resourceComponent.HostServerURL, _resourceComponent.FallbackHostServerURL);
                var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

                var initParameters = new HostPlayModeParameters();
                initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                initParameters.CacheFileSystemParameters = cacheFileSystemParams;
                initializationOperation = package.InitializeAsync(initParameters);
            }
            // WebGL运行模式
            else if (_resourceComponent.PlayMode == PlayMode.WebPlayMode)
            {
                IRemoteServices remoteServices = new RemoteServices(_resourceComponent.HostServerURL, _resourceComponent.FallbackHostServerURL);
                var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                var webRemoteFileSystemParams = FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

                var initParameters = new WebPlayModeParameters();
                initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
                initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
                initializationOperation = package.InitializeAsync(initParameters);
            }
            else
            {
                throw new InvalidOperationException();
            }

            initializationOperation.Completed += op =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Initialize package '{packageName}' succeed.");
                    initPackageCallbacks.InitPackageComplete?.Invoke(packageName);
                }
                else
                {
                    Log.Error($"Initialize package '{packageName}' failure: {op.Error}.");
                    initPackageCallbacks.InitPackageFailure?.Invoke(packageName, op.Error, op);
                }
            };
        }

        public override bool CheckAssetNameValid(string packageName, string assetName)
        {
            var package = YooAssets.GetPackage(packageName);
            return package.CheckLocationValid(assetName);
        }

        public override bool IsNeedDownloadFromRemote(AssetInfo assetInfo)
        {
            var package = YooAssets.GetPackage(assetInfo.PackageName);
            return package.IsNeedDownloadFromRemote(assetInfo.Reference as YooAsset.AssetInfo);
        }

        public override AssetInfo GetAssetInfo(string packageName, string assetName)
        {
            var package = YooAssets.GetPackage(packageName);
            var assetInfo = package.GetAssetInfo(assetName);
            return new AssetInfo(assetInfo.PackageName, assetInfo.AssetType, assetName, assetInfo.AssetPath, assetInfo.Error, assetInfo);
        }

        public override IResourcePackageDownloader CreatePackageDownloader(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            var downloader = package.CreateResourceDownloader(_resourceComponent.DownloadingMaxNum, _resourceComponent.FailedTryAgain);
            return new YooAssetResourcePackageDownloader(downloader);
        }

        public override void UnloadScene(string sceneAssetName, object sceneAsset, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            var sceneHandle = sceneAsset as YooAsset.SceneHandle;
            var unloadOperation = sceneHandle.UnloadAsync();
            unloadOperation.Completed += op =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Unload scene '{sceneAssetName}' succeed.");
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
