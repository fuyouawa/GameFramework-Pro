using System;
using YooAsset;

namespace GameMain.Runtime
{
    public static class YooAssetsHelper
    {
        public static ResourcePackage GetPackage(string packageName)
        {
            var package = YooAssets.TryGetPackage(packageName);
            if (package == null)
            {
                throw new ArgumentException($"Can not found package: {packageName}", nameof(packageName));
            }
            return package;
        }
    }
}
