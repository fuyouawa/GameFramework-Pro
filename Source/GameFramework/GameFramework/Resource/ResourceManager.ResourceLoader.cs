using System;
using System.Collections.Generic;
using GameFramework.ObjectPool;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private class ResourceLoader
        {
            private readonly ResourceManager m_ResourceManager;
            private readonly TaskPool<LoadResourceTaskBase> m_TaskPool;
            private readonly Dictionary<string, object> m_SceneNameToAssetMap;
            private readonly Dictionary<string, object> m_AssetPathToAssetMap;

            public ResourceLoader(ResourceManager resourceManager)
            {
                m_ResourceManager = resourceManager;
                m_TaskPool = new TaskPool<LoadResourceTaskBase>();
                m_SceneNameToAssetMap = new Dictionary<string, object>();
                m_AssetPathToAssetMap = new Dictionary<string, object>();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_TaskPool.Update(elapseSeconds, realElapseSeconds);
            }

            public void Shutdown()
            {
                m_TaskPool.Shutdown();
            }

            public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
            {
            }


            /// <summary>
            /// 增加加载资源代理辅助器。
            /// </summary>
            /// <param name="loadResourceAgentHelper">要增加的加载资源代理辅助器。</param>
            /// <param name="readOnlyPath">资源只读区路径。</param>
            /// <param name="readWritePath">资源读写区路径。</param>
            public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper,
                string readOnlyPath, string readWritePath)
            {
                LoadResourceAgent agent =
                    new LoadResourceAgent(loadResourceAgentHelper, this, readOnlyPath, readWritePath);
                m_TaskPool.AddAgent(agent);
            }

            /// <summary>
            /// 异步加载资源。
            /// </summary>
            /// <param name="packageName">资源包名称</param>
            /// <param name="assetName">要加载资源的名称。</param>
            /// <param name="assetType">要加载资源的类型。</param>
            /// <param name="priority">加载资源的优先级。</param>
            /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
            /// <param name="userData">用户自定义数据。</param>
            public void LoadAsset(string packageName, string assetName, Type assetType, int priority,
                LoadAssetCallbacks loadAssetCallbacks,
                object userData)
            {
                var key = $"{packageName}/{assetName}";
                if (m_AssetPathToAssetMap.TryGetValue(key, out object asset))
                {
                    loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(assetName, asset, 0, userData);
                    return;
                }

                LoadAssetTask loadAssetTask = LoadAssetTask.Create(packageName, assetName, assetType, priority,
                    new LoadAssetCallbacks((name, o, duration, data) =>
                    {
                        m_AssetPathToAssetMap[key] = o;
                        loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(name, o, duration, data);
                    }, loadAssetCallbacks.LoadAssetFailureCallback), userData);

                m_TaskPool.AddTask(loadAssetTask);
            }

            public void UnloadAsset(object asset)
            {
                //TODO UnloadAsset
            }

            /// <summary>
            /// 异步加载场景。
            /// </summary>
            /// <param name="packageName">资源包名称</param>
            /// <param name="sceneAssetName">要加载场景的名称。</param>
            /// <param name="priority">加载场景的优先级。</param>
            /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
            /// <param name="userData">用户自定义数据。</param>
            public void LoadScene(string packageName, string sceneAssetName, int priority,
                LoadSceneCallbacks loadSceneCallbacks,
                object userData)
            {
                var callbacks = new LoadSceneCallbacks(
                    (name, asset, duration, data) =>
                    {
                        m_SceneNameToAssetMap[name] = asset;
                        loadSceneCallbacks.LoadSceneSuccessCallback?.Invoke(sceneAssetName, asset, duration, data);
                    }, loadSceneCallbacks.LoadSceneFailureCallback);

                LoadSceneTask loadSceneTask =
                    LoadSceneTask.Create(packageName, sceneAssetName, priority, callbacks, userData);
                m_TaskPool.AddTask(loadSceneTask);
            }

            /// <summary>
            /// 异步卸载场景。
            /// </summary>
            /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
            /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
            /// <param name="userData">用户自定义数据。</param>
            public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
            {
                if (m_ResourceManager.m_ResourceHelper == null)
                {
                    throw new GameFrameworkException("You must set resource helper first.");
                }

                if (m_SceneNameToAssetMap.TryGetValue(sceneAssetName, out object asset))
                {
                    m_ResourceManager.m_ResourceHelper.UnloadScene(sceneAssetName, asset, unloadSceneCallbacks,
                        userData);
                }
                else
                {
                    throw new GameFrameworkException($"The scene asset '{sceneAssetName}' is not loaded.");
                }
            }
        }
    }
}
