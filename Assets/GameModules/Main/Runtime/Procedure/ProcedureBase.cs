using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            OnEnterAsync(procedureOwner).Forget();
        }

        protected virtual UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            OnLeaveAsync(procedureOwner, isShutdown).Forget();
        }

        protected virtual UniTask OnLeaveAsync(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            return UniTask.CompletedTask;
        }
    }
}
