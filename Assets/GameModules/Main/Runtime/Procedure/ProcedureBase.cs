using System;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public abstract class ProcedureBase : GameFramework.Procedure.ProcedureBase
    {
        protected virtual bool EnableAutoUpdateLoadingPhasesContext => true;
        protected virtual bool EnableAutoUpdateLoadingUISpinnerBox => true;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            EnterAsync(procedureOwner).Forget();
        }

        private async UniTask EnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            if (GameEntry.Context.Contains(Constant.Context.LoadingPhasesIndex))
            {
                var phaseCount = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesCount);
                var phaseIndex = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesIndex);

                if (EnableAutoUpdateLoadingPhasesContext)
                {
                    GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, phaseIndex + 1);
                }

                if (EnableAutoUpdateLoadingUISpinnerBox && GameEntry.UI.IsSpinnerBoxShowing())
                {
                    // Ensure that the previous phase is update completely.
                    await GameEntry.UI.UpdateSpinnerBoxAsync(phaseIndex / (float)phaseCount);

                    if (LoadingSpinnerDescriptionGetter != null)
                    {
                        // Update the description of the current phase.
                        GameEntry.UI.UpdateSpinnerBoxAsync(
                            () => LoadingSpinnerDescriptionGetter(phaseIndex, phaseCount),
                            phaseIndex / (float)phaseCount).Forget();
                    }
                }
            }

            await OnEnterAsync(procedureOwner);
        }

        protected virtual Func<int, int, string> LoadingSpinnerDescriptionGetter => GetLoadingSpinnerDescription;

        protected virtual string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return null;
        }

        protected virtual UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            return UniTask.CompletedTask;
        }

        protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
            LeaveAsync(procedureOwner, isShutdown).Forget();
        }

        private async UniTask LeaveAsync(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            await OnLeaveAsync(procedureOwner, isShutdown);
        }

        protected virtual UniTask OnLeaveAsync(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
        {
            return UniTask.CompletedTask;
        }
    }
}
