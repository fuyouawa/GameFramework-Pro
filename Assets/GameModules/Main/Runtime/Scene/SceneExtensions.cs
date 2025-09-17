using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain.Runtime
{
    public static class SceneExtensions
    {
        private static readonly Dictionary<string, UniTaskCompletionSource<Scene>> SceneLoadCompletedTcsByAssetPath =
            new Dictionary<string, UniTaskCompletionSource<Scene>>();

        private static readonly Dictionary<string, UniTaskCompletionSource> SceneUnloadCompletedTcsByAssetPath =
            new Dictionary<string, UniTaskCompletionSource>();

        static SceneExtensions()
        {
            GameEntry.Event.Subscribe<LoadSceneSuccessEventArgs>(OnEvent);
            GameEntry.Event.Subscribe<LoadSceneFailureEventArgs>(OnEvent);
            GameEntry.Event.Subscribe<UnloadSceneSuccessEventArgs>(OnEvent);
            GameEntry.Event.Subscribe<UnloadSceneFailureEventArgs>(OnEvent);
        }

        public static UniTask<Scene> LoadSceneAsync(this SceneComponent sceneComponent,
            string sceneAssetName,
            string customPackageName = "",
            LoadSceneMode sceneMode = LoadSceneMode.Single,
            LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
        {
            var packageName = string.IsNullOrEmpty(customPackageName)
                ? sceneComponent.CurrentPackageName
                : customPackageName;

            var key = $"{packageName}/{sceneAssetName}";

            if (SceneLoadCompletedTcsByAssetPath.TryGetValue(key, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<Scene>();
            SceneLoadCompletedTcsByAssetPath[key] = tcs;

            sceneComponent.LoadScene(sceneAssetName, customPackageName, Constant.AssetPriority.SceneAsset,
                new LoadSceneParameters()
                {
                    SceneMode = sceneMode,
                    PhysicsMode = physicsMode
                });

            return tcs.Task;
        }

        public static UniTask UnloadSceneAsync(this SceneComponent sceneComponent,
            string sceneAssetName,
            string customPackageName = "")
        {
            var packageName = string.IsNullOrEmpty(customPackageName)
                ? sceneComponent.CurrentPackageName
                : customPackageName;
            var key = $"{packageName}/{sceneAssetName}";
            if (SceneUnloadCompletedTcsByAssetPath.TryGetValue(key, out var tcs))
            {
                return tcs.Task;
            }
            tcs = new UniTaskCompletionSource();
            SceneUnloadCompletedTcsByAssetPath[key] = tcs;
            sceneComponent.UnloadScene(sceneAssetName, customPackageName);
            return tcs.Task;
        }

        private static void OnEvent(object sender, LoadSceneSuccessEventArgs e)
        {
            var path = $"{e.PackageName}/{e.SceneAssetName}";
            var tcs = SceneLoadCompletedTcsByAssetPath[path];

            if (e.SceneAsset is not Scene scene)
            {
                throw new Exception($"Scene asset '{path}' is not a scene.");
            }

            tcs.TrySetResult(scene);
            SceneLoadCompletedTcsByAssetPath.Remove(path);
        }

        private static void OnEvent(object sender, LoadSceneFailureEventArgs e)
        {
            var path = $"{e.PackageName}/{e.SceneAssetName}";
            var tcs = SceneLoadCompletedTcsByAssetPath[path];
            tcs.TrySetException(new Exception(e.ErrorMessage));
            SceneLoadCompletedTcsByAssetPath.Remove(path);
        }

        private static void OnEvent(object sender, UnloadSceneSuccessEventArgs e)
        {
            var path = $"{e.PackageName}/{e.SceneAssetName}";
            var tcs = SceneUnloadCompletedTcsByAssetPath[path];
            tcs.TrySetResult();
            SceneUnloadCompletedTcsByAssetPath.Remove(path);
        }

        private static void OnEvent(object sender, UnloadSceneFailureEventArgs e)
        {
            var path = $"{e.PackageName}/{e.SceneAssetName}";
            var tcs = SceneUnloadCompletedTcsByAssetPath[path];

            if (e.UserData is not UnloadSceneOperation operation)
            {
                throw new Exception("e.UserData is not UnloadSceneOperation.");
            }
            tcs.TrySetException(new Exception(operation.Error));
            SceneUnloadCompletedTcsByAssetPath.Remove(path);
        }
    }
}
