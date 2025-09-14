using System;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;

namespace GameMain.Runtime
{
    public class ProcedureStartGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }
    }
}
