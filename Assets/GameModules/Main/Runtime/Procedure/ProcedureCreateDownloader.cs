using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureCreateDownloader : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            var packageName = GameEntry.Context.Get<string>(Constant.Context.InitializePackageName);
            Log.Debug($"Create downloader for package '{packageName}'");

            var package = YooAssets.GetPackage(packageName);
            var downloader = package.CreateResourceDownloader(
                GameEntry.Resource.DownloadingMaxNum,
                GameEntry.Resource.FailedTryAgain);

            GameEntry.Context.Set(Constant.Context.PackageDownloader, downloader);
            if (downloader.TotalDownloadCount == 0)
            {
                Log.Debug("Not found any download files !");
                ChangeState<ProcedureDownloadOver>(procedureOwner);
            }
            else
            {
                Log.Debug($"Found total {downloader.TotalDownloadCount} files that need download ！");

                var size = downloader.TotalDownloadBytes / 1024f / 1024f;
                size = Mathf.Clamp(size, 0.01f, float.MaxValue);

                var index = await GameEntry.UI.ShowMessageBoxAsync(
                    $"找到更新补丁文件，数量：{downloader.TotalDownloadCount}，大小：{size:F2}MB。\n是否下载？",
                    UIMessageBoxType.Tip, UIMessageBoxButtons.YesNo);

                if (index == 0)
                {
                    ChangeState<ProcedureDownloadFiles>(procedureOwner);
                }
                else
                {
                    ChangeState<ProcedureEndGame>(procedureOwner);
                }
            }
        }
    }
}
