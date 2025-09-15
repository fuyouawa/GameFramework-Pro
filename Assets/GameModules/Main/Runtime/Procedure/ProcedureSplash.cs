using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 闪屏。
    /// </summary>
    public class ProcedureSplash : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            GameEntry.Context.Set(Constant.Context.LoadingPhasesCount, 8);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, -1);

            await GameEntry.UI.BeginSpinnerBoxAsync("开始加载流程", 0);
            ChangeState<ProcedureInitPackage>(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            // The standard game start phases count is 8.
            // ProcedureInitPackage => ProcedureUpdateVersion => ProcedureUpdateManifest => ProcedureCreateDownloader =>
            // ProcedureDownloadFiles => ProcedureDownloadOver => ProcedurePreload => ProcedureLoadAssembly =>
            // ProcedureStartGame
            GameEntry.Context.Set(Constant.Context.LoadingPhasesCount, 8);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, 0);

        }
    }
}
