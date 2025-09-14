//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayMode = GameFramework.Resource.PlayMode;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 资源组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Game Framework/Resource")]
    public sealed class ResourceComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private IResourceManager m_ResourceManager = null;
        // private EventComponent m_EventComponent = null;

        /// <summary>
        /// 资源系统运行模式。
        /// </summary>
        [SerializeField] private PlayMode m_PlayMode = PlayMode.EditorSimulateMode;

        /// <summary>
        /// 下载文件校验等级。
        /// </summary>
        [SerializeField] private FileVerifyLevel m_FileVerifyLevel = FileVerifyLevel.Middle;

        [SerializeField] private ReadWritePathType m_ReadWritePathType = ReadWritePathType.Unspecified;

        /// <summary>
        /// 资源包名称。
        /// </summary>
        [SerializeField] private string m_DefaultPackageName = "DefaultPackage";

        /// <summary>
        /// 设置异步系统参数，每帧执行消耗的最大时间切片（单位：毫秒）
        /// </summary>
        [SerializeField] private long m_Milliseconds = 30;

        [SerializeField] private float m_AssetAutoReleaseInterval = 60f;

        [SerializeField] private int m_AssetCapacity = 64;

        [SerializeField] private float m_AssetExpireTime = 60f;

        [SerializeField] private int m_AssetPriority = 0;

        [SerializeField] private float m_MinUnloadUnusedAssetsInterval = 60f;

        [SerializeField] private float m_MaxUnloadUnusedAssetsInterval = 300f;

        [SerializeField] private bool m_UseSystemUnloadUnusedAssets = true;

        [SerializeField] private int m_DownloadingMaxNum = 10;

        [SerializeField] private int m_FailedTryAgain = 3;

        [SerializeField] private LoadResourceAgentHelperBase m_LoadResourceAgentHelper = null;
        [SerializeField] private ResourceHelperBase m_ResourceHelper = null;

        private float m_LastUnloadUnusedAssetsOperationElapseSeconds = 0f;

        public PlayMode PlayMode => m_PlayMode;
        public FileVerifyLevel FileVerifyLevel => m_FileVerifyLevel;

        public float LastUnloadUnusedAssetsOperationElapseSeconds => m_LastUnloadUnusedAssetsOperationElapseSeconds;

        public long Milliseconds => m_Milliseconds;

        public string DefaultPackageName => m_DefaultPackageName;

        public float MaxUnloadUnusedAssetsInterval => m_MaxUnloadUnusedAssetsInterval;

        public int DownloadingMaxNum => m_DownloadingMaxNum;

        public int FailedTryAgain => m_FailedTryAgain;

        /// <summary>
        /// 获取当前资源适用的游戏版本号。
        /// </summary>
        public string ApplicableGameVersion => m_ResourceManager.ApplicableGameVersion;

        /// <summary>
        /// 获取当前内部资源版本号。
        /// </summary>
        public int InternalResourceVersion => m_ResourceManager.InternalResourceVersion;

        /// <summary>
        /// 获取资源只读路径。
        /// </summary>
        public string ReadOnlyPath => m_ResourceManager.ReadOnlyPath;

        /// <summary>
        /// 获取资源读写路径。
        /// </summary>
        public string ReadWritePath => m_ResourceManager.ReadWritePath;

        /// <summary>
        /// 获取或设置资源对象池自动释放可释放对象的间隔秒数。
        /// </summary>
        public float AssetAutoReleaseInterval
        {
            get { return m_ResourceManager.AssetAutoReleaseInterval; }
            set { m_ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval = value; }
        }

        /// <summary>
        /// 资源服务器地址。
        /// </summary>
        public string HostServerURL { get; set; }

        public string FallbackHostServerURL { get; set; }

        /// <summary>
        /// 获取或设置资源对象池的容量。
        /// </summary>
        public int AssetCapacity
        {
            get { return m_ResourceManager.AssetCapacity; }
            set { m_ResourceManager.AssetCapacity = m_AssetCapacity = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池对象过期秒数。
        /// </summary>
        public float AssetExpireTime
        {
            get { return m_ResourceManager.AssetExpireTime; }
            set { m_ResourceManager.AssetExpireTime = m_AssetExpireTime = value; }
        }

        /// <summary>
        /// 获取或设置资源对象池的优先级。
        /// </summary>
        public int AssetPriority
        {
            get { return m_ResourceManager.AssetPriority; }
            set { m_ResourceManager.AssetPriority = m_AssetPriority = value; }
        }

        public string CurrentPackageName
        {
            get => m_ResourceManager.CurrentPackageName;
            set => m_ResourceManager.CurrentPackageName = value;
        }

        public string PackageVersion { get; set; }
        public bool IsInitialized { get; private set; }

        private void Start()
        {
            BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
            if (baseComponent == null)
            {
                Log.Fatal("Base component is invalid.");
                return;
            }

            m_ResourceManager = GameFrameworkEntry.GetModule<IResourceManager>();
            if (m_ResourceManager == null)
            {
                Log.Fatal("Resource component is invalid.");
                return;
            }

            if (m_PlayMode == PlayMode.EditorSimulateMode)
            {
                Log.Debug(
                    "During this run, Game Framework will use editor resource files, which you should validate first.");
#if !UNITY_EDITOR
                m_PlayMode = PlayMode.OfflinePlayMode;
#endif
            }

            m_ResourceManager.SetReadOnlyPath(Application.streamingAssetsPath);
            if (m_ReadWritePathType == ReadWritePathType.TemporaryCache)
            {
                m_ResourceManager.SetReadWritePath(Application.temporaryCachePath);
            }
            else
            {
                if (m_ReadWritePathType == ReadWritePathType.Unspecified)
                {
                    m_ReadWritePathType = ReadWritePathType.PersistentData;
                }

                m_ResourceManager.SetReadWritePath(Application.persistentDataPath);
            }

            m_ResourceManager.AddLoadResourceAgentHelper(m_LoadResourceAgentHelper);
            m_ResourceManager.SetResourceHelper(m_ResourceHelper);

            m_ResourceManager.CurrentPackageName = m_DefaultPackageName;
            m_ResourceManager.PlayMode = m_PlayMode;
            m_ResourceManager.FileVerifyLevel = m_FileVerifyLevel;
            m_ResourceManager.Milliseconds = m_Milliseconds;
            m_ResourceManager.AssetAutoReleaseInterval = m_AssetAutoReleaseInterval;
            m_ResourceManager.AssetCapacity = m_AssetCapacity;
            m_ResourceManager.AssetExpireTime = m_AssetExpireTime;
            m_ResourceManager.AssetPriority = m_AssetPriority;
            IsInitialized = true;
            Log.Debug($"ResourceComponent Run Mode：{m_PlayMode}");
        }

        public void RequestPackageVersion(RequestPackageVersionCallbacks requestPackageVersionCallbacks,
            string customPackageName = "",
            object userData = null)
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            m_ResourceManager.RequestPackageVersion(requestPackageVersionCallbacks, userData);
        }

        public void UpdatePackageManifest(string packageVersion,
            UpdatePackageManifestCallbacks updatePackageManifestCallbacks,
            string customPackageName = "", object userData = null)
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            m_ResourceManager.UpdatePackageManifest(packageVersion, updatePackageManifestCallbacks, userData);
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasAssetResult HasAsset(string assetName, string customPackageName = "")
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            return m_ResourceManager.HasAsset(assetName);
        }

        public IResourcePackageDownloader CreatePackageDownloader(string packageName)
        {
            return m_ResourceManager.CreatePackageDownloader(packageName);
        }

        public IResourcePackageDownloader GetPackageDownloader(string packageName)
        {
            return m_ResourceManager.GetPackageDownloader(packageName);
        }

        /// <summary>
        /// 获取资源信息
        /// </summary>
        /// <param name="assetName">要获取资源信息的名称。</param>
        /// <returns></returns>
        public AssetInfo GetAssetInfo(string assetName, string customPackageName = "")
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            return m_ResourceManager.GetAssetInfo(assetName);
        }

        public AssetInfo[] GetAssetInfos(string[] tags, string customPackageName = "")
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            return m_ResourceManager.GetAssetInfos(tags);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="priority">加载资源的优先级。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(
            string assetName,
            LoadAssetCallbacks loadAssetCallbacks,
            string customPackageName = "",
            Type assetType = null,
            int? priority = null,
            object userData = null)
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            m_ResourceManager.LoadAsset(assetName, loadAssetCallbacks, assetType, priority, userData);
        }

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            if (asset == null)
            {
                return;
            }

            m_ResourceManager.UnloadAsset(asset);
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(
            string sceneAssetName,
            LoadSceneCallbacks loadSceneCallbacks,
            string customPackageName = "",
            int? priority = null,
            object userData = null)
        {
            if (!string.IsNullOrEmpty(customPackageName))
            {
                CurrentPackageName = customPackageName;
            }

            m_ResourceManager.LoadScene(sceneAssetName, loadSceneCallbacks, priority, userData);
        }

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要卸载场景资源的名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks,
            object userData = null)
        {
            m_ResourceManager.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }

        // public void ClearAllCacheFiles(
        //     FileClearMode fileClearMode,
        //     ClearAllCacheFilesCallbacks clearAllCacheFilesCallbacks,
        //     object userData = null)
        // {
        //     m_ResourceManager.ClearAllCacheFiles(fileClearMode, clearAllCacheFilesCallbacks, userData);
        // }
        //
        // public void ClearPackageCacheFiles(
        //     string packageName,
        //     FileClearMode fileClearMode,
        //     ClearPackageCacheFilesCallbacks clearPackageCacheFilesCallbacks,
        //     object userData = null)
        // {
        //     m_ResourceManager.ClearPackageCacheFiles(packageName, fileClearMode, clearPackageCacheFilesCallbacks, userData);
        // }
    }
}
