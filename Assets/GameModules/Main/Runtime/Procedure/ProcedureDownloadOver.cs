using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        private bool _needClearCache;

        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            Log.Debug("DownloadOver");

            var phaseCount = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesCount);
            var phaseIndex = GameEntry.Context.Get<int>(Constant.Context.LoadingPhasesIndex);
            GameEntry.Context.Set(Constant.Context.LoadingPhasesIndex, phaseIndex + 1);

            await GameEntry.UI.UpdateSpinnerBoxAsync(phaseIndex + 1 / (float)phaseCount);
            ChangeState<ProcedurePreload>(procedureOwner);
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
