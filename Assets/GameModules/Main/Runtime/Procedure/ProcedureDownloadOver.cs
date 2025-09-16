using System;
using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        private bool _needClearCache;

        protected override Func<int, int, string> LoadingSpinnerDescriptionGetter => null;

        protected override UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            Log.Debug("DownloadOver");
            ChangeState<ProcedurePreload>(procedureOwner);
            return UniTask.CompletedTask;
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
