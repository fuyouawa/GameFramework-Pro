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
    /// <summary>
    /// 流程 => 用户尝试更新静态版本
    /// </summary>
    public class ProcedureUpdateVersion : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;

            base.OnEnter(procedureOwner);

            // UILoadMgr.Show(UIDefine.UILoadUpdate, $"更新静态版本文件...");

            //检查设备是否能够访问互联网
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                Log.Warning("The device is not connected to the network");
                // UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.Label_Net_UnReachable);
                // UILoadTip.ShowMessageBox(LoadText.Instance.Label_Net_UnReachable, MessageShowType.TwoButton,
                //     LoadStyle.StyleEnum.Style_Retry,
                //     GetStaticVersion().Forget,
                //     () => { ChangeState<ProcedureInitResources>(procedureOwner); });
            }

            // UILoadMgr.Show(UIDefine.UILoadUpdate, LoadText.Instance.Label_RequestVersionIng);

            // 用户尝试更新静态版本。
            GameEntry.Resource.RequestPackageVersion(new RequestPackageVersionCallbacks(OnRequestPackageVersionSuccess, OnRequestPackageVersionFailure));
        }

        private void OnRequestPackageVersionSuccess(string packageName, string packageVersion)
        {
            GameEntry.Resource.PackageVersion = packageVersion;
            // Log.Debug($"Updated package Version : from {GameModule.Resource.GetPackageVersion()} to {operation.PackageVersion}");
            ChangeState<ProcedureUpdateManifest>(_procedureOwner);
        }

        private void OnRequestPackageVersionFailure(string packageName, string error)
        {
            Log.Error(error);

            // UILoadTip.ShowMessageBox($"用户尝试更新静态版本失败！点击确认重试 \n \n <color=#FF0000>原因{error}</color>", MessageShowType.TwoButton,
            //     LoadStyle.StyleEnum.Style_Retry
            //     , () => { ChangeState<ProcedureUpdateVersion>(_procedureOwner); }, UnityEngine.Application.Quit);
        }
    }
}
