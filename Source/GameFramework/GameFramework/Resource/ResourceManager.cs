//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameFramework.Resource
{
    /// <summary>
    /// 资源管理器。
    /// </summary>
    internal sealed partial class ResourceManager : GameFrameworkModule, IResourceManager
    {
        private string m_ReadOnlyPath;
        private string m_ReadWritePath;
        private string m_ApplicableGameVersion;
        private int m_InternalResourceVersion;
        private IResourceHelper m_ResourceHelper;

        private readonly ResourceLoader m_ResourceLoader;
        private readonly Dictionary<string, AssetInfo> m_AssetInfosCache;
        private readonly Dictionary<(string packageName, string[] tags), AssetInfo[]> m_AssetInfosCacheByTags;
        private readonly Dictionary<string, IResourcePackageDownloader> m_PackageDownloaders;
        private readonly List<string> m_InitializedPackageNames;

        /// <summary>
        /// 初始化资源管理器的新实例。
        /// </summary>
        public ResourceManager()
        {
            m_ReadOnlyPath = null;
            m_ReadWritePath = null;
            m_ApplicableGameVersion = null;
            m_InternalResourceVersion = 0;
            m_ResourceLoader = new ResourceLoader(this);
            m_AssetInfosCache = new Dictionary<string, AssetInfo>();
            m_AssetInfosCacheByTags = new Dictionary<(string packageName, string[] tags), AssetInfo[]>();
            m_PackageDownloaders = new Dictionary<string, IResourcePackageDownloader>();
            m_InitializedPackageNames = new List<string>();
        }

        /// <summary>
        /// 获取游戏框架模块优先级。
        /// </summary>
        /// <remarks>优先级较高的模块会优先轮询，并且关闭操作会后进行。</remarks>
        internal override int Priority
        {
            get { return 3; }
        }

        public string ApplicableGameVersion => m_ApplicableGameVersion;
        public int InternalResourceVersion => m_InternalResourceVersion;
        public PlayMode PlayMode { get; set; }
        public FileVerifyLevel FileVerifyLevel { get; set; }
        public int DownloadingMaxNum { get; set; }
        public int FailedTryAgain { get; set; }

        /// <summary>
        /// 获取资源只读区路径。
        /// </summary>
        public string ReadOnlyPath
        {
            get { return m_ReadOnlyPath; }
        }

        /// <summary>
        /// 获取资源读写区路径。
        /// </summary>
        public string ReadWritePath
        {
            get { return m_ReadWritePath; }
        }

        public long Milliseconds { get; set; }
        public float AssetAutoReleaseInterval { get; set; }
        public int AssetCapacity { get; set; }
        public float AssetExpireTime { get; set; }
        public int AssetPriority { get; set; }
        public string CurrentPackageName { get; set; }

        /// <summary>
        /// 资源管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds, float realElapseSeconds)
        {
            m_ResourceLoader.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 关闭并清理资源管理器。
        /// </summary>
        internal override void Shutdown()
        {
            m_ResourceLoader.Shutdown();
        }

        /// <summary>
        /// 设置资源只读区路径。
        /// </summary>
        /// <param name="readOnlyPath">资源只读区路径。</param>
        public void SetReadOnlyPath(string readOnlyPath)
        {
            if (string.IsNullOrEmpty(readOnlyPath))
            {
                throw new GameFrameworkException("Read-only path is invalid.");
            }

            m_ReadOnlyPath = readOnlyPath;
        }

        /// <summary>
        /// 设置资源读写区路径。
        /// </summary>
        /// <param name="readWritePath">资源读写区路径。</param>
        public void SetReadWritePath(string readWritePath)
        {
            if (string.IsNullOrEmpty(readWritePath))
            {
                throw new GameFrameworkException("Read-write path is invalid.");
            }

            m_ReadWritePath = readWritePath;
        }

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
        {
            if (objectPoolManager == null)
            {
                throw new GameFrameworkException("Object pool manager is invalid.");
            }
        }

        /// <summary>
        /// 添加加载资源代理辅助器。
        /// </summary>
        /// <param name="loadResourceAgentHelper">要添加的加载资源代理辅助器。</param>
        public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)
        {
            if (string.IsNullOrEmpty(m_ReadOnlyPath))
            {
                throw new GameFrameworkException("Read-only path is invalid.");
            }

            if (string.IsNullOrEmpty(m_ReadWritePath))
            {
                throw new GameFrameworkException("Read-write path is invalid.");
            }

            m_ResourceLoader.AddLoadResourceAgentHelper(loadResourceAgentHelper, m_ReadOnlyPath, m_ReadWritePath);
        }

        public IResourcePackageDownloader CreatePackageDownloader(string packageName)
        {
            var downloader = m_ResourceHelper.CreatePackageDownloader(packageName);
            m_PackageDownloaders[packageName] = downloader;
            return downloader;
        }

        public IResourcePackageDownloader GetPackageDownloader(string packageName)
        {
            if (m_PackageDownloaders.TryGetValue(packageName, out IResourcePackageDownloader downloader))
            {
                return downloader;
            }

            return null;
        }

        public void SetResourceHelper(IResourceHelper resourceHelper)
        {
            m_ResourceHelper = resourceHelper;
        }

        public void Initialize()
        {
            m_ResourceHelper.Initialize();
        }

        public void InitPackage(string packageName, InitPackageCallbacks initPackageCallbacks)
        {
            m_ResourceHelper.InitPackage(packageName, new InitPackageCallbacks(name =>
            {
                m_InitializedPackageNames.Add(packageName);
                initPackageCallbacks.InitPackageSuccess?.Invoke(packageName);
            }, initPackageCallbacks.InitPackageFailure));
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasAssetResult HasAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }

            AssetInfo assetInfo = GetAssetInfo(assetName);
            if (assetInfo == null)
            {
                return HasAssetResult.NotExist;
            }

            if (!m_ResourceHelper.CheckAssetNameValid(CurrentPackageName, assetName))
            {
                return HasAssetResult.NotExist;
            }

            if (m_ResourceHelper.IsNeedDownloadFromRemote(assetInfo))
            {
                return HasAssetResult.AssetOnline;
            }

            return HasAssetResult.AssetOnDisk;
        }

        public AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }

            string key = $"{CurrentPackageName}/{assetName}";
            if (m_AssetInfosCache.TryGetValue(key, out AssetInfo assetInfo))
            {
                return assetInfo;
            }

            assetInfo = m_ResourceHelper.GetAssetInfo(CurrentPackageName, assetName);
            m_AssetInfosCache[key] = assetInfo;
            return assetInfo;
        }

        public AssetInfo[] GetAssetInfos(string[] tags)
        {
            if (tags == null || tags.Length == 0)
            {
                throw new GameFrameworkException("Tags is empty.");
            }

            var key = (CurrentPackageName, tags);
            if (m_AssetInfosCacheByTags.TryGetValue(key, out AssetInfo[] assetInfos))
            {
                return assetInfos;
            }

            assetInfos = m_ResourceHelper.GetAssetInfos(CurrentPackageName, tags);
            m_AssetInfosCacheByTags[key] = assetInfos;
            return assetInfos;
        }

        public void RequestPackageVersion(RequestPackageVersionCallbacks requestPackageVersionCallbacks,
            object userData = null)
        {
            m_ResourceHelper.RequestPackageVersion(CurrentPackageName, requestPackageVersionCallbacks, userData);
        }

        public void UpdatePackageManifest(string packageVersion,
            UpdatePackageManifestCallbacks updatePackageManifestCallbacks, object userData = null)
        {
            m_ResourceHelper.UpdatePackageManifest(CurrentPackageName, packageVersion, updatePackageManifestCallbacks,
                userData);
        }

        public void LoadAsset(
            string assetName,
            LoadAssetCallbacks loadAssetCallbacks,
            Type assetType = null,
            int? priority = null,
            object userData = null)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new GameFrameworkException("Asset name is invalid.");
            }

            if (loadAssetCallbacks == null)
            {
                throw new GameFrameworkException("Load asset callbacks is invalid.");
            }

            m_ResourceLoader.LoadAsset(CurrentPackageName, assetName, assetType, priority ?? Constant.DefaultPriority,
                loadAssetCallbacks, userData);
        }

        /// <summary>
        /// 卸载资源。
        /// </summary>
        /// <param name="asset">要卸载的资源。</param>
        public void UnloadAsset(object asset)
        {
            if (asset == null)
            {
                throw new GameFrameworkException("Asset is invalid.");
            }

            m_ResourceLoader.UnloadAsset(asset);
        }

        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, int? priority = null,
            object userData = null)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new GameFrameworkException("Scene asset name is invalid.");
            }

            if (loadSceneCallbacks == null)
            {
                throw new GameFrameworkException("Load scene callbacks is invalid.");
            }

            m_ResourceLoader.LoadScene(CurrentPackageName, sceneAssetName, priority ?? Constant.DefaultPriority,
                loadSceneCallbacks, userData);
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
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                throw new GameFrameworkException("Scene asset name is invalid.");
            }

            if (unloadSceneCallbacks == null)
            {
                throw new GameFrameworkException("Unload scene callbacks is invalid.");
            }

            m_ResourceLoader.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData);
        }

        public void ClearAllCacheFiles(
            FileClearMode fileClearMode,
            ClearAllCacheFilesCallbacks clearAllCacheFilesCallbacks,
            object userData = null)
        {
            int clearingPackageCount = m_InitializedPackageNames.Count;
            bool hasError = false;
            var clearPackageCacheFilesCallbacks = new ClearPackageCacheFilesCallbacks(
                OnClearPackageCacheFilesSuccess,
                OnClearPackageCacheFilesFailure);

            foreach (var packageName in m_InitializedPackageNames)
            {
                ClearPackageCacheFiles(packageName, fileClearMode, clearPackageCacheFilesCallbacks, userData);
            }

            void OnClearPackageCacheFilesSuccess(string packageName)
            {
                if (clearingPackageCount <= 0)
                {
                    throw new GameFrameworkException();
                }
                clearingPackageCount--;
                clearAllCacheFilesCallbacks.ClearPackageCacheFilesSuccess?.Invoke(packageName);
                if (clearingPackageCount == 0)
                {
                    clearAllCacheFilesCallbacks.ClearAllCacheFilesComplete?.Invoke(hasError);
                }
            }

            void OnClearPackageCacheFilesFailure(string packageName, string errorMessage)
            {
                if (clearingPackageCount <= 0)
                {
                    throw new GameFrameworkException();
                }
                clearingPackageCount--;
                hasError = true;
                clearAllCacheFilesCallbacks.ClearPackageCacheFilesFailure?.Invoke(packageName, errorMessage);
                if (clearingPackageCount == 0)
                {
                    clearAllCacheFilesCallbacks.ClearAllCacheFilesComplete?.Invoke(hasError);
                }
            }
        }

        public void ClearPackageCacheFiles(string packageName,
            FileClearMode fileClearMode,
            ClearPackageCacheFilesCallbacks clearPackageCacheFilesCallbacks,
            object userData = null)
        {
            m_ResourceHelper.ClearPackageCacheFiles(packageName, fileClearMode, clearPackageCacheFilesCallbacks, userData);
        }
    }
}
