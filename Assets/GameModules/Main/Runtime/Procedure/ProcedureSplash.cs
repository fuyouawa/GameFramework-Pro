using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 闪屏。
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            // The standard game start phases count is 10.
            // ProcedureInitPackage => ProcedureUpdateVersion => ProcedureUpdateManifest => ProcedureCreateDownloader =>
            // ProcedureDownloadFiles => ProcedureDownloadOver => ProcedurePreload => ProcedureLoadAssembly =>
            // ProcedureStartGame => ProcedureLoadScene
            GameEntry.Context.Set(Constant.Context.LoadingPhasesCount, 10);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, 0);

            await GameEntry.UI.BeginSpinnerBoxAsync("开始加载流程", 0, 1f);

            GameEntry.Context.Set(Constant.Context.InitializePackageName, GameEntry.Resource.DefaultPackageName);
            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}
