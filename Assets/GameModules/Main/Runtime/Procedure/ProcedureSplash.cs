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
            GameEntry.Context.Set(Constant.Context.LoadingPhasesCount, Constant.Procedure.SplashPhaseCount);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, 0);

            await GameEntry.UI.BeginSpinnerBoxAsync("开始加载流程", 0, 1f);

            GameEntry.Context.Set(Constant.Context.InitializePackageName, GameEntry.Resource.DefaultPackageName);
            ChangeState<ProcedureInitPackage>(procedureOwner);
        }
    }
}
