namespace GameFramework.Resource
{
    public delegate void InitPackageCompleteCallback(string packageName);
    public delegate void InitPackageFailureCallback(string packageName, string errorMessage, object userData);

    public class InitPackageCallbacks
    {
        private readonly InitPackageCompleteCallback m_InitPackageComplete;
        private readonly InitPackageFailureCallback m_InitPackageFailure;

        public InitPackageCallbacks(InitPackageCompleteCallback initPackageCompleteCallback,
            InitPackageFailureCallback initPackageFailureCallback)
        {
            m_InitPackageComplete = initPackageCompleteCallback;
            m_InitPackageFailure = initPackageFailureCallback;
        }

        public InitPackageCompleteCallback InitPackageComplete => m_InitPackageComplete;
        public InitPackageFailureCallback InitPackageFailure => m_InitPackageFailure;
    }

    public interface IResourceHelper
    {
        void Initialize();
        void InitPackage(string packageName, InitPackageCallbacks initPackageCallbacks);

        bool CheckAssetNameValid(string packageName, string assetName);
        bool IsNeedDownloadFromRemote(AssetInfo assetInfo);
        AssetInfo GetAssetInfo(string packageName, string assetName);

        IResourcePackageDownloader CreatePackageDownloader(string packageName);

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void UnloadScene(string sceneAssetName, object sceneAsset, UnloadSceneCallbacks unloadSceneCallbacks, object userData);
    }
}
