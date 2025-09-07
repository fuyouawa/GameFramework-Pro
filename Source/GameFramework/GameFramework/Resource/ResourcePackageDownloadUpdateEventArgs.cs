namespace GameFramework.Resource
{
    public class ResourcePackageDownloadUpdateEventArgs : GameFrameworkEventArgs
    {
        public ResourcePackageDownloadUpdateEventArgs()
        {
            PackageName = null;
            Progress = 0f;
            TotalDownloadCount = 0;
            CurrentDownloadCount = 0;
            TotalDownloadBytes = 0;
            CurrentDownloadBytes = 0;
        }

        /// <summary>
        /// 所属包裹名称
        /// </summary>
        public string PackageName { get; private set; }

        /// <summary>
        /// 下载进度 (0-1f)
        /// </summary>
        public float Progress { get; private set; }

        /// <summary>
        /// 下载文件总数
        /// </summary>
        public int TotalDownloadCount { get; private set; }

        /// <summary>
        /// 当前完成的下载文件数量
        /// </summary>
        public int CurrentDownloadCount { get; private set; }

        /// <summary>
        /// 下载数据总大小（单位：字节）
        /// </summary>
        public long TotalDownloadBytes { get; private set; }

        /// <summary>
        /// 当前完成的下载数据大小（单位：字节）
        /// </summary>
        public long CurrentDownloadBytes { get; private set; }

        public static ResourcePackageDownloadUpdateEventArgs Create(string packageName, float progress,
            int totalDownloadCount, int currentDownloadCount, long totalDownloadBytes, long currentDownloadBytes)
        {
            ResourcePackageDownloadUpdateEventArgs resourcePackageDownloadUpdateEventArgs = ReferencePool.Acquire<ResourcePackageDownloadUpdateEventArgs>();
            resourcePackageDownloadUpdateEventArgs.PackageName = packageName;
            resourcePackageDownloadUpdateEventArgs.Progress = progress;
            resourcePackageDownloadUpdateEventArgs.TotalDownloadCount = totalDownloadCount;
            resourcePackageDownloadUpdateEventArgs.CurrentDownloadCount = currentDownloadCount;
            resourcePackageDownloadUpdateEventArgs.TotalDownloadBytes = totalDownloadBytes;
            resourcePackageDownloadUpdateEventArgs.CurrentDownloadBytes = currentDownloadBytes;
            return resourcePackageDownloadUpdateEventArgs;
        }

        public override void Clear()
        {
            PackageName = null;
            Progress = 0f;
            TotalDownloadCount = 0;
            CurrentDownloadCount = 0;
            TotalDownloadBytes = 0;
            CurrentDownloadBytes = 0;
        }
    }
}
