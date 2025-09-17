using System;
using System.Collections.Generic;
using System.Linq;
using GameFramework.ObjectPool;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private class ResourceLoader
        {
            private readonly ResourceManager m_ResourceManager;
            private readonly TaskPool<LoadResourceTaskBase> m_TaskPool;
            private readonly Dictionary<string, AssetObject> m_AssetPathToAssetMap;

            public ResourceLoader(ResourceManager resourceManager)
            {
                m_ResourceManager = resourceManager;
                m_TaskPool = new TaskPool<LoadResourceTaskBase>();
                m_AssetPathToAssetMap = new Dictionary<string, AssetObject>();
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

            public void RegisterAsset(string packageName, string assetName, AssetObject assetObject)
            {
                var key = $"{packageName}/{assetName}";
                if (!m_AssetPathToAssetMap.TryAdd(key, assetObject))
                {
                    throw new GameFrameworkException($"Asset '{key}' has been registered.");
                }
            }

            public AssetObject GetAssetObject(string packageName, string assetName)
            {
                var key = $"{packageName}/{assetName}";
                if (m_AssetPathToAssetMap.TryGetValue(key, out AssetObject assetObject))
                {
                    return assetObject;
                }
                throw new GameFrameworkException($"Asset '{key}' is not loaded.");
            }

            public AssetObject GetAssetObject(object asset)
            {
                if (asset == null)
                {
                    throw new GameFrameworkException("Asset is invalid.");
                }

                foreach (var assetObject in m_AssetPathToAssetMap.Values)
                {
                    if (assetObject.Asset == asset)
                    {
                        return assetObject;
                    }
                }

                throw new GameFrameworkException($"Asset '{asset}' is not loaded.");
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
                LoadResourceAgent agent = new LoadResourceAgent(loadResourceAgentHelper, this, readOnlyPath, readWritePath);
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
                if (m_AssetPathToAssetMap.TryGetValue(key, out AssetObject assetObject))
                {
                    if (assetObject.IsScene)
                    {
                        throw new GameFrameworkException($"Asset '{key}' is a scene asset, use LoadScene instead.");
                    }

                    loadAssetCallbacks.LoadAssetSuccessCallback?.Invoke(packageName, assetName, assetObject.Asset, 0, userData);
                    return;
                }

                LoadAssetTask loadAssetTask = LoadAssetTask.Create(packageName, assetName, assetType, priority,
                    loadAssetCallbacks, userData);

                m_TaskPool.AddTask(loadAssetTask);
            }

            public void UnloadAsset(object asset)
            {
                var pair = m_AssetPathToAssetMap.FirstOrDefault(pair => pair.Value.Asset == asset);
                if (pair.Value == null)
                {
                    return;
                }

                if (pair.Value.IsScene)
                {
                    throw new GameFrameworkException($"Asset '{pair.Key}' is a scene asset, use UnloadScene instead.");
                }

                m_ResourceManager.m_ResourceHelper.UnloadAsset(pair.Value);
                m_AssetPathToAssetMap.Remove(pair.Key);
                ReferencePool.Release(pair.Value);
            }

            public void UnloadAsset(string packageName, string assetName)
            {
                var key = $"{packageName}/{assetName}";

                if (m_AssetPathToAssetMap.TryGetValue(key, out AssetObject assetObject))
                {
                    if (assetObject.IsScene)
                    {
                        throw new GameFrameworkException($"Asset '{key}' is a scene asset, use UnloadScene instead.");
                    }

                    m_ResourceManager.m_ResourceHelper.UnloadAsset(assetObject);
                    m_AssetPathToAssetMap.Remove(key);
                    ReferencePool.Release(assetObject);
                }
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
                LoadSceneTask loadSceneTask = LoadSceneTask.Create(packageName, sceneAssetName, priority,
                    loadSceneCallbacks, userData);
                m_TaskPool.AddTask(loadSceneTask);
            }

            /// <summary>
            /// 异步卸载场景。
            /// </summary>
            /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
            /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
            /// <param name="userData">用户自定义数据。</param>
            public void UnloadScene(string packageName, string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
            {
                if (m_ResourceManager.m_ResourceHelper == null)
                {
                    throw new GameFrameworkException("You must set resource helper first.");
                }

                var key = $"{packageName}/{sceneAssetName}";
                if (m_AssetPathToAssetMap.TryGetValue(key, out AssetObject assetObject))
                {
                    if (!assetObject.IsScene)
                    {
                        throw new GameFrameworkException($"Asset '{key}' is not a scene asset, use UnloadAsset instead.");
                    }
                    m_ResourceManager.m_ResourceHelper.UnloadScene(packageName, sceneAssetName, assetObject, unloadSceneCallbacks,
                        userData);

                    m_AssetPathToAssetMap.Remove(key);
                    ReferencePool.Release(assetObject);
                }
                else
                {
                    throw new GameFrameworkException($"The scene asset '{sceneAssetName}' is not loaded.");
                }
            }
        }
    }
}
