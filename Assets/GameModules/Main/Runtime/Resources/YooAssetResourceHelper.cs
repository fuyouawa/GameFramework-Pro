using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;
using AssetInfo = GameFramework.Resource.AssetInfo;

namespace GameMain
{
    public class YooAssetResourceHelper : ResourceHelperBase
    {
        public override void Initialize()
        {
            var manager = GameEntry.GetComponent<ResourceComponent>();

            // 初始化资源系统
            if (!YooAssets.Initialized)
            {
                YooAssets.Initialize(new ResourceLogger());
            }
            YooAssets.SetOperationSystemMaxTimeSlice(manager.Milliseconds);

            // 创建默认的资源包
            string packageName = manager.DefaultPackageName;
            var defaultPackage = YooAssets.TryGetPackage(packageName);
            if (defaultPackage == null)
            {
                defaultPackage = YooAssets.CreatePackage(packageName);
                YooAssets.SetDefaultPackage(defaultPackage);
            }
        }

        public override void InitPackage(string packageName, InitPackageCallbacks initPackageCallbacks)
        {
            throw new System.NotImplementedException();
        }

        public override bool CheckAssetNameValid(string packageName, string assetName)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsNeedDownloadFromRemote(AssetInfo assetInfo)
        {
            throw new System.NotImplementedException();
        }

        public override AssetInfo GetAssetInfo(string packageName, string assetName)
        {
            throw new System.NotImplementedException();
        }

        public override void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            throw new System.NotImplementedException();
        }
    }
}
