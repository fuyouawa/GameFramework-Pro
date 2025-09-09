//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework.Resource;
using UnityEngine;

namespace UnityGameFramework.Runtime
{
    /// <summary>
    /// 资源辅助器基类。
    /// </summary>
    public abstract class ResourceHelperBase : MonoBehaviour, IResourceHelper
    {
        public abstract void Initialize();
        public abstract void InitPackage(string packageName, InitPackageCallbacks initPackageCallbacks);

        public abstract bool CheckAssetNameValid(string packageName, string assetName);

        public abstract bool IsNeedDownloadFromRemote(AssetInfo assetInfo);

        public abstract AssetInfo GetAssetInfo(string packageName, string assetName);
        public abstract AssetInfo[] GetAssetInfos(string packageName, string[] tags);

        public abstract void RequestPackageVersion(string packageName, RequestPackageVersionCallbacks requestPackageVersionCallbacks,
            object userData = null);

        public abstract void UpdatePackageManifest(string packageName, string packageVersion, UpdatePackageManifestCallbacks updatePackageManifestCallbacks,
            object userData = null);

        public abstract IResourcePackageDownloader CreatePackageDownloader(string packageName);

        public abstract void ClearPackageCacheFiles(string packageName, FileClearMode fileClearMode,
            ClearPackageCacheFilesCallbacks clearPackageCacheFilesCallbacks, object userData = null);

        public abstract void UnloadScene(string sceneAssetName, object sceneAsset, UnloadSceneCallbacks unloadSceneCallbacks,
            object userData);
    }
}
