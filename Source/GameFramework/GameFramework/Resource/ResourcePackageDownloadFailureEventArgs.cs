namespace GameFramework.Resource
{
    public class ResourcePackageDownloadFailureEventArgs : GameFrameworkEventArgs
    {
        public ResourcePackageDownloadFailureEventArgs()
        {
            PackageName = null;
            FileName = null;
            ErrorMessage = null;
        }

        /// <summary>
        /// 所属包裹名称
        /// </summary>
        public string PackageName { get; private set; }

        /// <summary>
        /// 下载失败的文件名称
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        public static ResourcePackageDownloadFailureEventArgs Create(string packageName, string fileName,
            string errorMessage)
        {
            ResourcePackageDownloadFailureEventArgs resourcePackageDownloadFailureEventArgs = ReferencePool.Acquire<ResourcePackageDownloadFailureEventArgs>();
            resourcePackageDownloadFailureEventArgs.PackageName = packageName;
            resourcePackageDownloadFailureEventArgs.FileName = fileName;
            resourcePackageDownloadFailureEventArgs.ErrorMessage = errorMessage;
            return resourcePackageDownloadFailureEventArgs;
        }

        public override void Clear()
        {
            PackageName = null;
            FileName = null;
            ErrorMessage = null;
        }
    }
}
