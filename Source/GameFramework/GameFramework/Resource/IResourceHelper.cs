namespace GameFramework.Resource
{
    public interface IResourceHelper
    {
        void Initialize();
        void InitPackage(string packageName, InitPackageCompleteCallback initPackageCompleteCallback);

        bool CheckAssetNameValid(string packageName, string assetName);
        bool IsNeedDownloadFromRemote(AssetInfo assetInfo);
        AssetInfo GetAssetInfo(string packageName, string assetName);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string packageName, string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData);
    }
}
