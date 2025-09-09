using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureDownloadFile : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;

        private float _lastUpdateDownloadedSize;

        private float CurrentSpeed
        {
            get
            {
                float interval = Time.deltaTime;
                var downloader = GameEntry.Resource.GetPackageDownloader();
                var sizeDiff = downloader.CurrentDownloadBytes - _lastUpdateDownloadedSize;
                _lastUpdateDownloadedSize = downloader.CurrentDownloadBytes;
                var speed = (float)Math.Floor(sizeDiff / interval);
                return speed;
            }
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;

            Log.Debug("开始下载更新文件！");

            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"开始下载更新文件...");

            var downloader = GameEntry.Resource.GetPackageDownloader();

            downloader.DownloadFailure += OnDownloadFailure;
            downloader.DownloadSuccess += OnDownloadSuccess;
            downloader.DownloadUpdate += OnDownloadUpdate;

            downloader.BeginDownload();
        }

        private void OnDownloadUpdate(object sender, ResourcePackageDownloadUpdateEventArgs e)
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

        private void OnDownloadSuccess(object sender, ResourcePackageDownloadSuccessEventArgs e)
        {
            ChangeState<ProcedureDownloadOver>(_procedureOwner);
        }

        private void OnDownloadFailure(object sender, ResourcePackageDownloadFailureEventArgs e)
        {
            // UILoadTip.ShowMessageBox($"Failed to download file : {fileName}", MessageShowType.TwoButton,
            //     LoadStyle.StyleEnum.Style_Default
            //     , () => { ChangeState<ProcedureCreateDownloader>(_procedureOwner); }, UnityEngine.Application.Quit);
        }
    }
}
