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

        public override void Initialize()
        {
            // 初始化资源系统
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new ResourceLogger());
            }

            YooAssets.SetOperationSystemMaxTimeSlice(ResourceComponent.Milliseconds);

            // 创建默认的资源包
            string packageName = ResourceComponent.DefaultPackageName;
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
            switch (ResourceComponent.PlayMode)
            {
                case PlayMode.EditorSimulateMode:
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var editorFileSystemParams = FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.EditorFileSystemParameters = editorFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // 单机运行模式
                case PlayMode.OfflinePlayMode:
                {
                    var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    var initParameters = new OfflinePlayModeParameters();
                    initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // 联机运行模式
                case PlayMode.HostPlayMode:
                {
                    IRemoteServices remoteServices = new RemoteServices(ResourceComponent.HostServerURL,
                        ResourceComponent.FallbackHostServerURL);
                    var cacheFileSystemParams = FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                    var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

                    var initParameters = new HostPlayModeParameters();
                    initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                    initParameters.CacheFileSystemParameters = cacheFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // WebGL运行模式
                case PlayMode.WebPlayMode:
                {
                    IRemoteServices remoteServices = new RemoteServices(ResourceComponent.HostServerURL,
                        ResourceComponent.FallbackHostServerURL);
                    var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                    var webRemoteFileSystemParams =
                        FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

                    var initParameters = new WebPlayModeParameters();
                    initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
                    initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                default:
                    throw new InvalidOperationException();
            }

            initializationOperation.Completed += op =>
            {
                if (op.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Initialize package '{packageName}' succeed.");
                    initPackageCallbacks.InitPackageSuccess?.Invoke(packageName);
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
            return new AssetInfo(assetInfo.PackageName, assetInfo.AssetType, assetName, assetInfo.Error, assetInfo);
        }

        public override AssetInfo[] GetAssetInfos(string packageName, string[] tags)
        {
            var package = YooAssets.GetPackage(packageName);
            var assetInfos = package.GetAssetInfos(tags);
            return assetInfos.Select(assetInfo => new AssetInfo(assetInfo.PackageName, assetInfo.AssetType,
                assetInfo.AssetPath, assetInfo.Error, assetInfo)).ToArray();
        }

        public override void RequestPackageVersion(string packageName, RequestPackageVersionCallbacks requestPackageVersionCallbacks,
            object userData = null)
        {
            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            operation.Completed += op =>
            {
                if (operation.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Request package '{packageName}' version '{operation.PackageVersion}' succeed.");
                    requestPackageVersionCallbacks.RequestPackageVersionSuccessCallback?.Invoke(packageName, operation.PackageVersion);
                }
                else
                {
                    Log.Error($"Request package '{packageName}' version failure: {operation.Error}.");
                    requestPackageVersionCallbacks.RequestPackageVersionFailureCallback?.Invoke(packageName, operation.Error);
                }
            };
        }

        public override void UpdatePackageManifest(string packageName, string packageVersion, UpdatePackageManifestCallbacks updatePackageManifestCallbacks,
            object userData = null)
        {
            var package = YooAssets.GetPackage(packageName);
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            operation.Completed += op =>
            {
                if (operation.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Update package '{packageName}' manifest succeed.");
                    updatePackageManifestCallbacks.UpdatePackageManifestSuccessCallback?.Invoke(packageName);
                }
                else
                {
                    Log.Error($"Update package '{packageName}' manifest failure: {operation.Error}.");
                    updatePackageManifestCallbacks.UpdatePackageManifestFailureCallback?.Invoke(packageName, operation.Error);
                }
            };
        }

        public override IResourcePackageDownloader CreatePackageDownloader(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            var downloader = package.CreateResourceDownloader(ResourceComponent.DownloadingMaxNum,
                ResourceComponent.FailedTryAgain);
            return new YooAssetResourcePackageDownloader(downloader);
        }

        public override void ClearPackageCacheFiles(string packageName, FileClearMode fileClearMode,
            ClearPackageCacheFilesCallbacks clearPackageCacheFilesCallbacks, object userData = null)
        {
            var package = YooAssets.GetPackage(packageName);
            var operation = package.ClearCacheFilesAsync(fileClearMode switch
            {
                FileClearMode.ClearAllBundleFiles => EFileClearMode.ClearAllBundleFiles,
                FileClearMode.ClearUnusedBundleFiles => EFileClearMode.ClearUnusedBundleFiles,
                FileClearMode.ClearBundleFilesByTags => EFileClearMode.ClearBundleFilesByTags,
                FileClearMode.ClearAllManifestFiles => EFileClearMode.ClearAllManifestFiles,
                FileClearMode.ClearUnusedManifestFiles => EFileClearMode.ClearUnusedManifestFiles,
                _ => throw new ArgumentOutOfRangeException(nameof(fileClearMode), fileClearMode, null)
            }, userData);

            operation.Completed += op =>
            {
                if (operation.Status == EOperationStatus.Succeed)
                {
                    Log.Info($"Clear package '{packageName}' cache files by mode '{fileClearMode}' succeed.");
                    clearPackageCacheFilesCallbacks.ClearPackageCacheFilesSuccess?.Invoke(packageName);
                }
                else
                {
                    Log.Error($"Clear package '{packageName}' cache files by mode '{fileClearMode}' failure: {operation.Error}.");
                    clearPackageCacheFilesCallbacks.ClearPackageCacheFilesFailure?.Invoke(packageName, operation.Error);
                }
            };
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
