namespace GameFramework.Resource
{
    public class AssetObject : IReference
    {
        public object Asset { get; private set; }
        public bool IsScene { get; private set; }
        public object UserData { get; private set; }

        public static AssetObject Create(object asset, bool isScene, object userData)
        {
            var assetObject = ReferencePool.Acquire<AssetObject>();
            assetObject.Asset = asset;
            assetObject.IsScene = isScene;
            assetObject.UserData = userData;
            return assetObject;
        }

        public override int GetHashCode()
        {
            return Asset.GetHashCode();
        }

        public override string ToString()
        {
            return Asset.ToString();
        }

        public void Clear()
        {
            Asset = null;
            IsScene = false;
            UserData = null;
        }
    }
}
