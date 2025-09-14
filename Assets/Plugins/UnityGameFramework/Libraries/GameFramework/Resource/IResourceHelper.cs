namespace GameFramework.Resource
{
    public interface IResourceHelper
    {
        bool CheckAssetNameValid(string packageName, string assetName);
        bool IsNeedDownloadFromRemote(AssetInfo assetInfo);
        AssetInfo GetAssetInfo(string packageName, string assetName);
        AssetInfo[] GetAssetInfos(string packageName, string[] tags);

        void RequestPackageVersion(string packageName, RequestPackageVersionCallbacks requestPackageVersionCallbacks, object userData);
        void UpdatePackageManifest(string packageName, string packageVersion, UpdatePackageManifestCallbacks updatePackageManifestCallbacks, object userData);


        IResourcePackageDownloader CreatePackageDownloader(string packageName);

        // void ClearPackageCacheFiles(
        //     string packageName,
        //     FileClearMode fileClearMode,
        //     ClearPackageCacheFilesCallbacks clearPackageCacheFilesCallbacks,
        //     object userData = null);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string sceneAssetName, object sceneAsset, UnloadSceneCallbacks unloadSceneCallbacks, object userData);
    }
}
