using System;
using GameFramework.Resource;
using YooAsset;

namespace GameMain.Runtime
{
    public class YooAssetResourcePackageDownloader : IResourcePackageDownloader
    {
        private readonly DownloaderOperation _downloaderOperation;

        public event EventHandler<ResourcePackageDownloadSuccessEventArgs> DownloadSuccess;
        public event EventHandler<ResourcePackageDownloadUpdateEventArgs> DownloadUpdate;
        public event EventHandler<ResourcePackageDownloadFailureEventArgs> DownloadFailure;

        public YooAssetResourcePackageDownloader(DownloaderOperation downloaderOperation)
        {
            _downloaderOperation = downloaderOperation;
            _downloaderOperation.DownloadFinishCallback += data =>
            {
                if (data.Succeed)
                {
                    DownloadSuccess?.Invoke(this, ResourcePackageDownloadSuccessEventArgs.Create(data.PackageName));
                }
            };

            _downloaderOperation.DownloadUpdateCallback += data =>
            {
                DownloadUpdate?.Invoke(this,
                    ResourcePackageDownloadUpdateEventArgs.Create(data.PackageName, data.Progress,
                        data.TotalDownloadCount, data.CurrentDownloadCount, data.TotalDownloadBytes,
                        data.CurrentDownloadBytes));
            };

            _downloaderOperation.DownloadErrorCallback += data =>
            {
                DownloadFailure?.Invoke(this,
                    ResourcePackageDownloadFailureEventArgs.Create(data.PackageName, data.FileName, data.ErrorInfo));
            };
        }

        public int TotalDownloadCount => _downloaderOperation.TotalDownloadCount;
        public long TotalDownloadBytes => _downloaderOperation.TotalDownloadBytes;
        public int CurrentDownloadCount => _downloaderOperation.CurrentDownloadCount;
        public long CurrentDownloadBytes => _downloaderOperation.CurrentDownloadBytes;

        public void BeginDownload()
        {
            _downloaderOperation.BeginDownload();
        }

        public void PauseDownload()
        {
            _downloaderOperation.PauseDownload();
        }

        public void ResumeDownload()
        {
            _downloaderOperation.ResumeDownload();
        }

        public void CancelDownload()
        {
            _downloaderOperation.CancelDownload();
        }
    }
}
