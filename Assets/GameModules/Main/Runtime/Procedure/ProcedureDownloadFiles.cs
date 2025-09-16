using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureDownloadFiles : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;
        private ResourceDownloaderOperation _downloader;

        private long _lastUpdateDownloadedSize;

        private ResourceDownloaderOperation Downloader => _downloader ??=
            GameEntry.Context.Get<ResourceDownloaderOperation>(Constant.Context.PackageDownloader);

        private long CurrentSpeedBytes
        {
            get
            {
                var sizeDiff = Downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = Downloader.CurrentDownloadBytes;
                var speed = sizeDiff / (double)Time.deltaTime;
                return (long)speed;
            }
        }

        protected override bool EnableAutoUpdateLoadingUISpinnerBox => Downloader.TotalDownloadCount > 0;

        protected override UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            if (Downloader.TotalDownloadCount == 0)
            {
                ChangeState<ProcedureDownloadOver>(procedureOwner);
                return UniTask.CompletedTask;
            }

            _procedureOwner = procedureOwner;

            Downloader.DownloadFinishCallback += OnDownloadFinish;
            Downloader.DownloadErrorCallback += OnDownloadError;
            Downloader.DownloadUpdateCallback += OnDownloadUpdate;

            Downloader.BeginDownload();
            return UniTask.CompletedTask;
        }

        private void OnDownloadFinish(DownloaderFinishData data)
        {
            var phaseCount = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesCount);
            var phaseIndex = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesIndex);
            GameEntry.UI.UpdateSpinnerBoxAsync(phaseIndex / (float)phaseCount)
                .ContinueWith(() => ChangeState<ProcedureDownloadOver>(_procedureOwner))
                .Forget();
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return $"下载资源包“{Downloader.PackageName}”......\n" +
                   $"进度：{BytesToMb(Downloader.CurrentDownloadBytes)}/{BytesToMb(Downloader.TotalDownloadCount)}mb\n" +
                   $"速度：{BytesToMb(CurrentSpeedBytes)}mb/s";

            string BytesToMb(long bytes)
            {
                var mb = Mathf.Clamp(bytes / 1024f / 1024f, 0.01f, float.MaxValue);
                return mb.ToString("F");
            }
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
        }

        private void OnDownloadError(DownloadErrorData data)
        {
            Log.Error($"Download files failed: {data.ErrorInfo}");
            GameEntry.UI.ShowMessageBoxAsync($"下载资源失败，是否尝试重新下载？",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo)
                .ContinueWith(i =>
                {
                    if (i == 0)
                    {
                        ChangeState<ProcedureCreateDownloader>(_procedureOwner);
                    }
                    else
                    {
                        ChangeState<ProcedureEndGame>(_procedureOwner);
                    }
                })
                .Forget();
        }
    }
}
