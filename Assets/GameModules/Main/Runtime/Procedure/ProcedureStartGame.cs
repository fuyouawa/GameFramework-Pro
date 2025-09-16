using System;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public class ProcedureStartGame : ProcedureBase
    {
        protected override UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            return base.OnEnterAsync(procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return "加载开始界面......";
        }
    }
}
