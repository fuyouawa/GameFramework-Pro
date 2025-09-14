using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain.Runtime
{
    public static class ResourceExtensions
    {
        private static readonly LoadAssetCallbacks LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);

        private static readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> AssetLoadCompletedTcsByName =
            new Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>>();

        public static async UniTask InitializePackageAsync(this ResourceComponent resourceComponent,
            string packageName,
            PlayMode playMode)
        {
            var package = YooAssets.TryGetPackage(packageName);
            if (package is { InitializeStatus: EOperationStatus.Succeed })
            {
                return;
            }

            package = YooAssets.CreatePackage(packageName);

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            switch (playMode)
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
                    IRemoteServices remoteServices = new RemoteServices(resourceComponent.HostServerURL,
                        resourceComponent.FallbackHostServerURL);
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
                    IRemoteServices remoteServices = new RemoteServices(resourceComponent.HostServerURL,
                        resourceComponent.FallbackHostServerURL);
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

            await initializationOperation.ToUniTask();
        }

        public static async UniTask<T> LoadAssetAsync<T>(this ResourceComponent resourceComponent, string assetName,
            string customPackageName = "",
            int? priority = null,
            object userData = null)
            where T : UnityEngine.Object
        {
            var result = await LoadAssetAsync(resourceComponent, assetName, customPackageName, typeof(T), priority, userData);
            return (T)result;
        }

        public static UniTask<UnityEngine.Object> LoadAssetAsync(this ResourceComponent resourceComponent, string assetName,
            string customPackageName = "",
            Type assetType = null,
            int? priority = null,
            object userData = null)
        {
            if (AssetLoadCompletedTcsByName.TryGetValue(assetName, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<UnityEngine.Object>();
            AssetLoadCompletedTcsByName[assetName] = tcs;
            resourceComponent.LoadAsset(assetName, LoadAssetCallbacks, customPackageName, assetType, priority, userData);
            return tcs.Task;
        }

        private static void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var tcs = AssetLoadCompletedTcsByName[assetName];
            tcs.TrySetResult((UnityEngine.Object)asset);
            AssetLoadCompletedTcsByName.Remove(assetName);
        }

        private static void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string error, object userData)
        {
            var tcs = AssetLoadCompletedTcsByName[assetName];
            tcs.TrySetException(new Exception(error));
            AssetLoadCompletedTcsByName.Remove(assetName);
        }
    }
}
