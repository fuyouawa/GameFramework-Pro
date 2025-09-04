using System;

namespace GameFramework.Resource
{
    internal sealed partial class ResourceManager
    {
        private class ResourceLoader
        {
            private readonly ResourceManager m_ResourceManager;
            private readonly TaskPool<LoadResourceTaskBase> m_TaskPool;

            public ResourceLoader(ResourceManager resourceManager)
            {
                m_ResourceManager = resourceManager;
                m_TaskPool = new TaskPool<LoadResourceTaskBase>();
            }

            public void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_TaskPool.Update(elapseSeconds, realElapseSeconds);
            }

            public void Shutdown()
            {
                m_TaskPool.Shutdown();
            }


            /// <summary>
            /// 增加加载资源代理辅助器。
            /// </summary>
            /// <param name="loadResourceAgentHelper">要增加的加载资源代理辅助器。</param>
            /// <param name="readOnlyPath">资源只读区路径。</param>
            /// <param name="readWritePath">资源读写区路径。</param>
            public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper, string readOnlyPath, string readWritePath)
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
            public void LoadAsset(string packageName, string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks,
                object userData)
            {
                LoadAssetTask loadAssetTask = LoadAssetTask.Create(packageName, assetName, assetType, priority, loadAssetCallbacks, userData);
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
            public void LoadScene(string packageName, string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks,
                object userData)
            {
                LoadSceneTask loadSceneTask = LoadSceneTask.Create(packageName, sceneAssetName, priority, loadSceneCallbacks, userData);
                m_TaskPool.AddTask(loadSceneTask);
            }

            /// <summary>
            /// 异步卸载场景。
            /// </summary>
            /// <param name="packageName">资源包名称</param>
            /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
            /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
            /// <param name="userData">用户自定义数据。</param>
            public void UnloadScene(string packageName, string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
            {
                if (m_ResourceManager.m_ResourceHelper == null)
                {
                    throw new GameFrameworkException("You must set resource helper first.");
                }

                m_ResourceManager.m_ResourceHelper.UnloadScene(packageName, sceneAssetName, unloadSceneCallbacks, userData);
            }
        }
    }
}
