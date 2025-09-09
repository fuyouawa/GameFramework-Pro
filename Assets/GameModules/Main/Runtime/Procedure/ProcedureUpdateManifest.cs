using System;
using Cysharp.Threading.Tasks;
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
        private ProcedureOwner _procedureOwner;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            Log.Info("更新资源清单！！！");

            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"更新清单文件...");

            GameEntry.Resource.UpdatePackageManifest(GameEntry.Resource.PackageVersion,
                new UpdatePackageManifestCallbacks(OnUpdatePackageManifestSuccess, OnUpdatePackageManifestFailure));
        }

        private void OnUpdatePackageManifestSuccess(string packageName)
        {
            if (GameEntry.Resource.PlayMode == PlayMode.WebPlayMode)
            {
                ChangeState<ProcedurePreload>(_procedureOwner);
                return;
            }

            ChangeState<ProcedureCreateDownloader>(_procedureOwner);
        }

        private void OnUpdatePackageManifestFailure(string packageName, string error)
        {
            // UILoadTip.ShowMessageBox($"用户尝试更新清单失败！点击确认重试 \n \n <color=#FF0000>原因{operation.Error}</color>", MessageShowType.TwoButton,
            //     LoadStyle.StyleEnum.Style_Retry
            //     , () => { ChangeState<ProcedureUpdateManifest>(procedureOwner); }, UnityEngine.Application.Quit);
        }
    }
}
