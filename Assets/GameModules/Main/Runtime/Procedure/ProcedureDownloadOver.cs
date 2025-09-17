using System;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        private bool _needClearCache;

        protected override Func<int, int, string> LoadingSpinnerDescriptionGetter => null;

        protected override UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            Log.Debug("DownloadOver");
            ChangeState<ProcedurePreload>(procedureOwner);
            return UniTask.CompletedTask;
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
        }
    }
}
