using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureCreateDownloader : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"创建补丁下载器...");
            CreateDownloader(procedureOwner).Forget();
        }

        private async UniTaskVoid CreateDownloader(ProcedureOwner procedureOwner)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(0.5f));

            var downloader = GameEntry.Resource.CreatePackageDownloader(GameEntry.Resource.DefaultPackageName);

            if (downloader.TotalDownloadCount == 0)
            {
                Log.Debug("Not found any download files !");
                ChangeState<ProcedureDownloadOver>(procedureOwner);
            }
            else
            {
                //A total of 10 files were found that need to be downloaded
                Log.Debug($"Found total {downloader.TotalDownloadCount} files that need download ！");

                // 发现新更新文件后，挂起流程系统
                // 注意：开发者需要在下载前检测磁盘空间不足
                int totalDownloadCount = downloader.TotalDownloadCount;
                long totalDownloadBytes = downloader.TotalDownloadBytes;

                float sizeMb = totalDownloadBytes / 1048576f;
                sizeMb = UnityEngine.Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                string totalSizeMb = sizeMb.ToString("f1");

                // UILoadTip.ShowMessageBox($"Found update patch files, Total count {totalDownloadCount} Total size {totalSizeMb}MB", MessageShowType.TwoButton,
                //     LoadStyle.StyleEnum.Style_StartUpdate_Notice
                //     , () => { StartDownFile(procedureOwner: procedureOwner); }, UnityEngine.Application.Quit);
                StartDownFile(procedureOwner);
            }
        }

        void StartDownFile(ProcedureOwner procedureOwner)
        {
            ChangeState<ProcedureDownloadFile>(procedureOwner);
        }
    }
}
