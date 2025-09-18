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

        protected virtual Func<int, int, string> LoadingSpinnerDescriptionGetter => GetLoadingSpinnerDescription;

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
                    float percentage = phaseIndex / (float)phaseCount;
                    // Ensure that the previous phase is update completely.
                    await GameEntry.UI.UpdateSpinnerBoxAsync(percentage);

                    if (LoadingSpinnerDescriptionGetter != null)
                    {
                        // Update the description of the current phase.
                        GameEntry.UI.UpdateSpinnerBoxAsync(
                            () => LoadingSpinnerDescriptionGetter(phaseIndex, phaseCount),
                            percentage).Forget();
                    }
                }
            }

            await OnEnterAsync(procedureOwner);
        }

        protected virtual string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return null;
        }

        protected virtual UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            return UniTask.CompletedTask;
        }
    }
}
