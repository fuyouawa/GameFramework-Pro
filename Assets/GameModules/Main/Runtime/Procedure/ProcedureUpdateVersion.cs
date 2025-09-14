using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using PlayMode = GameFramework.Resource.PlayMode;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 用户尝试更新静态版本
    /// </summary>
    public class ProcedureUpdateVersion : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            if (GameEntry.Resource.PlayMode is not (PlayMode.EditorSimulateMode or PlayMode.OfflinePlayMode))
            {
                //检查设备是否能够访问互联网
                while (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    var i = await GameEntry.UI.ShowMessageBoxAsync("网络不可用，请检查网络链接。", UIMessageBoxType.Error,
                        UIMessageBoxButtons.OkCancel);
                    if (i == 1)
                    {
                        ChangeState<ProcedureEndGame>(procedureOwner);
                        return;
                    }
                }
            }

            var packageName = GameEntry.Context.Get<string>(Constant.Context.InitializePackageName);
            var packageVersion = await RequestPackageVersionWithRetryAsync(packageName);
            if (string.IsNullOrEmpty(packageVersion))
            {
                ChangeState<ProcedureEndGame>(procedureOwner);
                return;
            }

            GameEntry.Setting.SetString(Utility.Text.Format(Constant.Setting.PackageVersion, packageName), packageVersion);
            ChangeState<ProcedureUpdateManifest>(procedureOwner);
        }

        private async UniTask<string> RequestPackageVersionWithRetryAsync(string packageName, int retryCount = 0)
        {
            try
            {
                var packageVersion = await RequestPackageVersionAsync(packageName);
                Log.Debug($"Request package '{packageName}' version '{packageVersion}' success.");
                return packageVersion;
            }
            catch (Exception e)
            {
                Log.Error($"Update package '{packageName}' version failed: {e}");
                var result = await GameEntry.UI.ShowMessageBoxAsync($"请求获取资源包“{packageName}”版本失败，是否尝试重新请求",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数，游戏即将退出。", UIMessageBoxType.Error);
                        return null;
                    }

                    var packageVersion = await RequestPackageVersionWithRetryAsync(packageName, retryCount + 1);
                    if (!string.IsNullOrEmpty(packageVersion))
                    {
                        return packageVersion;
                    }
                }

                return null;
            }
        }

        private async UniTask<string> RequestPackageVersionAsync(string packageName)
        {
            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            await operation.ToUniTask();
            return operation.PackageVersion;
        }
    }
}
