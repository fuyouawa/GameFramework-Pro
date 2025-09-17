﻿using System;
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

        private static readonly Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> AssetLoadCompletedTcsByPath =
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
            var packageName = string.IsNullOrEmpty(customPackageName)
                ? resourceComponent.CurrentPackageName
                : customPackageName;

            var path = $"{packageName}/{assetName}";
            if (AssetLoadCompletedTcsByPath.TryGetValue(path, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<UnityEngine.Object>();
            AssetLoadCompletedTcsByPath[path] = tcs;
            resourceComponent.LoadAsset(assetName, LoadAssetCallbacks, customPackageName, assetType, priority, userData);
            return tcs.Task;
        }

        private static void OnLoadAssetSuccess(string packageName, string assetName, object asset, float duration, object userData)
        {
            var path = $"{packageName}/{assetName}";
            var tcs = AssetLoadCompletedTcsByPath[path];
            tcs.TrySetResult((UnityEngine.Object)asset);
            AssetLoadCompletedTcsByPath.Remove(path);
        }

        private static void OnLoadAssetFailure(string packageName, string assetName, LoadResourceStatus status, string error, object userData)
        {
            var path = $"{packageName}/{assetName}";
            var tcs = AssetLoadCompletedTcsByPath[path];
            tcs.TrySetException(new Exception(error));
            AssetLoadCompletedTcsByPath.Remove(path);
        }
    }
}
