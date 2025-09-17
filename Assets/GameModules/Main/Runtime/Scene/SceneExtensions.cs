using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static class SceneExtensions
    {
        private static readonly Dictionary<string, UniTaskCompletionSource<Scene>> SceneLoadCompletedTcsByAssetPath =
            new Dictionary<string, UniTaskCompletionSource<Scene>>();

        static SceneExtensions()
        {
            GameEntry.Event.Subscribe<LoadSceneSuccessEventArgs>(OnEvent);
            GameEntry.Event.Subscribe<LoadSceneFailureEventArgs>(OnEvent);
        }

        public static UniTask LoadSceneAsync(this SceneComponent sceneComponent,
            string sceneAssetName,
            string customPackageName = "",
            int? priority = null,
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
            
            sceneComponent.LoadScene(sceneAssetName, customPackageName, priority,
                new LoadSceneParameters()
                {
                    SceneMode = sceneMode,
                    PhysicsMode = physicsMode
                });

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

    }
}
