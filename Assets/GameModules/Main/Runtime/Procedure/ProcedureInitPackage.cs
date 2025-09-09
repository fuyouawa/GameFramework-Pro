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
    /// 流程 => 初始化Package。
    /// </summary>
    public class ProcedureInitPackage : ProcedureBase
    {
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //Fire Forget立刻触发UniTask初始化Package
            InitPackage(procedureOwner);
        }

        private void InitPackage(ProcedureOwner procedureOwner)
        {
            try
            {
                var package = YooAssets.TryGetPackage(GameEntry.Resource.DefaultPackageName);
                if (package != null && package.InitializeStatus == EOperationStatus.Succeed)
                {
                    OnInitPackageComplete(procedureOwner, GameEntry.Resource.DefaultPackageName);
                    return;
                }

                GameEntry.Resource.InitPackage(new InitPackageCallbacks(
                    name => OnInitPackageComplete(procedureOwner, name),
                    (name, error, data) => OnInitPackageFailure(procedureOwner, name, error)));
            }
            catch (Exception e)
            {
                OnInitPackageFailure(procedureOwner, GameEntry.Resource.DefaultPackageName, e.Message);
            }
        }

        private void OnInitPackageComplete(ProcedureOwner procedureOwner, string packageName)
        {
            // 编辑器模式。
            if (GameEntry.Resource.PlayMode == PlayMode.EditorSimulateMode)
            {
                Log.Debug("Editor resource mode detected.");
                ChangeState<ProcedurePreload>(procedureOwner);
            }
            // 单机模式。
            else if (GameEntry.Resource.PlayMode == PlayMode.OfflinePlayMode)
            {
                Log.Debug("Package resource mode detected.");
                ChangeState<ProcedureInitResources>(procedureOwner);
            }
            // 可更新模式。
            else if (GameEntry.Resource.PlayMode == PlayMode.HostPlayMode ||
                     GameEntry.Resource.PlayMode == PlayMode.WebPlayMode)
            {
                // 打开启动UI。
                // UILoadMgr.Show(UIDefine.UILoadUpdate);

                Log.Debug("Updatable resource mode detected.");
                ChangeState<ProcedureUpdateVersion>(procedureOwner);
            }
            else
            {
                Log.Error("UnKnow resource mode detected Please check???");
            }
        }

        private void OnInitPackageFailure(ProcedureOwner procedureOwner, string packageName, string error)
        {
            // 打开启动UI。
            // UILoadMgr.Show(UIDefine.UILoadUpdate);

            Log.Error($"Init package {packageName} failure: {error}.");

            // 打开启动UI。
            // UILoadMgr.Show(UIDefine.UILoadUpdate, $"资源初始化失败！");
            //
            // UILoadTip.ShowMessageBox($"资源初始化失败！点击确认重试 \n \n <color=#FF0000>原因{message}</color>", MessageShowType.TwoButton,
            //     LoadStyle.StyleEnum.Style_Retry
            //     , () => { Retry(procedureOwner); },
            //     GameModule.QuitApplication);
        }

        private void Retry(ProcedureOwner procedureOwner)
        {
            // 打开启动UI。
            // UILoadMgr.Show(UIDefine.UILoadUpdate, $"重新初始化资源中...");

            InitPackage(procedureOwner);
        }
    }
}
