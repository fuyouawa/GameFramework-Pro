namespace GameFramework.Resource
{
    public class ResourcePackageDownloadSuccessEventArgs : GameFrameworkEventArgs
    {
        public ResourcePackageDownloadSuccessEventArgs()
        {
            PackageName = null;
        }

        /// <summary>
        /// 所属包裹名称
        /// </summary>
        public string PackageName { get; private set; }

        public static ResourcePackageDownloadSuccessEventArgs Create(string packageName)
        {
            ResourcePackageDownloadSuccessEventArgs packageDownloadSuccessEventArgs = ReferencePool.Acquire<ResourcePackageDownloadSuccessEventArgs>();
            packageDownloadSuccessEventArgs.PackageName = packageName;
            return packageDownloadSuccessEventArgs;
        }

        public override void Clear()
        {
            PackageName = null;
        }
    }
}
