using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Resource;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class ResourceExtensions
    {
        private static readonly LoadAssetCallbacks LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);

        private static readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> AssetLoadTcsByName =
            new Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>>();

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
            if (AssetLoadTcsByName.TryGetValue(assetName, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<UnityEngine.Object>();
            AssetLoadTcsByName[assetName] = tcs;
            resourceComponent.LoadAsset(assetName, LoadAssetCallbacks, customPackageName, assetType, priority, userData);
            return tcs.Task;
        }

        private static void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
        {
            var tcs = AssetLoadTcsByName[assetName];
            tcs.TrySetResult((UnityEngine.Object)asset);
            AssetLoadTcsByName.Remove(assetName);
        }

        private static void OnLoadAssetFailure(string assetName, LoadResourceStatus status, string error, object userData)
        {
            var tcs = AssetLoadTcsByName[assetName];
            tcs.TrySetException(new Exception(error));
            AssetLoadTcsByName.Remove(assetName);
        }
    }
}
