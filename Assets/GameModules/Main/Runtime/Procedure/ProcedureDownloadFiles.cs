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

        private long CurrentSpeedBytes
        {
            get
            {
                var sizeDiff = _downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = _downloader.CurrentDownloadBytes;
                var speed = sizeDiff / (double)Time.deltaTime;
                return (long)speed;
            }
        }

        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            var phaseCount = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesCount);
            var phaseIndex = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesIndex);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, phaseIndex + 1);
            GameEntry.UI.UpdateSpinnerBoxAsync(GetDescription, phaseIndex / (float)phaseCount).Forget();

            if (_downloader.TotalDownloadCount == 0)
            {
                await GameEntry.UI.UpdateSpinnerBoxAsync(phaseIndex + 1 / (float)phaseCount);
                ChangeState<ProcedureDownloadOver>(procedureOwner);
                return;
            }

            _procedureOwner = procedureOwner;
            _downloader = GameEntry.Context.Get<ResourceDownloaderOperation>(Constant.Context.PackageDownloader);

            _downloader.DownloadFinishCallback += OnDownloadFinish;
            _downloader.DownloadErrorCallback += OnDownloadError;
            _downloader.DownloadUpdateCallback += OnDownloadUpdate;

            _downloader.BeginDownload();
        }

        private void OnDownloadFinish(DownloaderFinishData data)
        {
            var phaseCount = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesCount);
            var phaseIndex = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesIndex);
            GameEntry.UI.UpdateSpinnerBoxAsync(phaseIndex / (float)phaseCount)
                .ContinueWith(() => ChangeState<ProcedureDownloadOver>(_procedureOwner))
                .Forget();
        }

        private string GetDescription()
        {
            return $"下载资源包“{_downloader.PackageName}”......\n进度：{BytesToMb(_downloader.CurrentDownloadBytes)}/{BytesToMb(_downloader.TotalDownloadCount)}mb\n速度：{BytesToMb(CurrentSpeedBytes)}mb/s";

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
