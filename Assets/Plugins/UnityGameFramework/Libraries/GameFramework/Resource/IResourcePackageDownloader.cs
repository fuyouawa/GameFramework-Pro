using System;

namespace GameFramework.Resource
{
    public interface IResourcePackageDownloader
    {
        /// <summary>
        /// 统计的下载文件总数量
        /// </summary>
        int TotalDownloadCount { get; }

        /// <summary>
        /// 统计的下载文件的总大小
        /// </summary>
        long TotalDownloadBytes { get; }

        /// <summary>
        /// 当前已经完成的下载总数量
        /// </summary>
        int CurrentDownloadCount { get; }

        /// <summary>
        /// 当前已经完成的下载总大小
        /// </summary>
        long CurrentDownloadBytes { get; }

        event EventHandler<ResourcePackageDownloadSuccessEventArgs> DownloadSuccess;
        event EventHandler<ResourcePackageDownloadUpdateEventArgs> DownloadUpdate;
        event EventHandler<ResourcePackageDownloadFailureEventArgs> DownloadFailure;

        void BeginDownload();
        void PauseDownload();
        void ResumeDownload();
        void CancelDownload();
    }
}
