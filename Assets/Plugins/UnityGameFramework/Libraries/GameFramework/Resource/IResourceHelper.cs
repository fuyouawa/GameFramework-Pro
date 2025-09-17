namespace GameFramework.Resource
{
    public interface IResourceHelper
    {
        bool CheckAssetNameValid(string packageName, string assetName);
        bool IsNeedDownloadFromRemote(AssetInfo assetInfo);
        AssetInfo GetAssetInfo(string packageName, string assetName);
        AssetInfo[] GetAssetInfos(string packageName, string[] tags);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="packageName">资源包名称。</param>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string packageName, string sceneAssetName, AssetObject sceneAssetObject, UnloadSceneCallbacks unloadSceneCallbacks, object userData);

        void UnloadAsset(AssetObject assetObject);
    }
}
