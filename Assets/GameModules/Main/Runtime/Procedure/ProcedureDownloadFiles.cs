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

        private float _lastUpdateDownloadedSize;

        private float CurrentSpeed
        {
            get
            {
                float interval = Time.deltaTime;
                var sizeDiff = _downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = _downloader.CurrentDownloadBytes;
                var speed = (float)Math.Floor(sizeDiff / interval);
                return speed;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            _downloader = GameEntry.Context.Get<ResourceDownloaderOperation>(Constant.Context.PackageDownloader);

            _downloader.DownloadFinishCallback += OnDownloadFinish;
            _downloader.DownloadErrorCallback += OnDownloadError;
            _downloader.DownloadUpdateCallback += OnDownloadUpdate;

            _downloader.BeginDownload();
        }

        private void OnDownloadFinish(DownloaderFinishData data)
        {
            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }

        private void OnDownloadUpdate(DownloadUpdateData data)
        {
            // string currentSizeMb = (currentDownloadBytes / 1048576f).ToString("f1");
            // string totalSizeMb = (totalDownloadBytes / 1048576f).ToString("f1");
            // // UILoadMgr.Show(UIDefine.UILoadUpdate,$"{currentDownloadCount}/{totalDownloadCount} {currentSizeMb}MB/{totalSizeMb}MB");
            // string descriptionText = Utility.Text.Format("正在更新，已更新{0}，总更新{1}，已更新大小{2}，总更新大小{3}，更新进度{4}，当前网速{5}/s",
            //     currentDownloadCount.ToString(),
            //     totalDownloadCount.ToString(),
            //     Utility.File.GetByteLengthString(currentDownloadBytes),
            //     Utility.File.GetByteLengthString(totalDownloadBytes),
            //     GameModule.Resource.Downloader.Progress,
            //     Utility.File.GetLengthString((int)CurrentSpeed));
            // GameEvent.Send(StringId.StringToHash("DownProgress"), GameModule.Resource.Downloader.Progress);
            // UILoadMgr.Show(UIDefine.UILoadUpdate,descriptionText);
            //
            // int needTime = 0;
            // if (CurrentSpeed > 0)
            // {
            //     needTime = (int)((totalDownloadBytes - currentDownloadBytes) / CurrentSpeed);
            // }
            //
            // TimeSpan ts = new TimeSpan(0, 0, needTime);
            // string timeStr = ts.ToString(@"mm\:ss");
            // string updateProgress = Utility.Text.Format("剩余时间 {0}({1}/s)", timeStr, Utility.File.GetLengthString((int)CurrentSpeed));
            // Log.Info(updateProgress);
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
