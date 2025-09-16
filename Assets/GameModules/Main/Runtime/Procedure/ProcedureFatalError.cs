using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public class ProcedureFatalError : ProcedureBase
    {
        protected virtual bool EnableAutoUpdateLoadingPhasesContext => false;
        protected virtual bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            await GameEntry.UI.ShowMessageBoxAsync("出现严重异常，游戏即将退出。", UIMessageBoxType.Fatal);
            ChangeState<ProcedureEndGame>(procedureOwner);
        }
    }
}
