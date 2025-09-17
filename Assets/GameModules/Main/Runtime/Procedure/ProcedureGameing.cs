using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public class ProcedureGameing : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            if (GameEntry.UI.IsSpinnerBoxShowing())
            {
                await GameEntry.UI.UpdateSpinnerBoxAsync("加载完成", 1);
                await GameEntry.UI.EndSpinnerBoxAsync();
            }
        }
    }
}