using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 用户尝试更新清单
    /// </summary>
    public class ProcedureUpdateManifest : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            var packageName = GameEntry.Context.Get<string>(Constant.Context.InitializePackageName);
            if (await UpdatePackageManifestWithRetryAsync(packageName))
            {
                ChangeState<ProcedureCreateDownloader>(procedureOwner);
            }
            else
            {
                ChangeState<ProcedureEndGame>(procedureOwner);
            }
        }

        private async UniTask<bool> UpdatePackageManifestWithRetryAsync(string packageName, int retryCount = 0)
        {
            try
            {
                await UpdatePackageManifestAsync(packageName);
                Log.Debug($"Update package '{packageName}' manifest success.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Update package '{packageName}' manifest failed: {e}");
                var result = await GameEntry.UI.ShowMessageBoxAsync($"更新资源包“{packageName}”清单失败，是否尝试重新更新",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数，游戏即将退出。", UIMessageBoxType.Error);
                        return false;
                    }

                    if (await UpdatePackageManifestWithRetryAsync(packageName, retryCount + 1))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private async UniTask UpdatePackageManifestAsync(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);

            var packageVersion = GameEntry.Setting.GetString(Utility.Text.Format(Constant.Setting.PackageVersion, packageName));
            var operation = package.UpdatePackageManifestAsync(packageVersion);
            await operation.ToUniTask();
        }
    }
}
