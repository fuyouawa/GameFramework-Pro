using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;

namespace GameMain.Runtime
{
    public class ProcedureEndGame : ProcedureBase
    {
        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }
    }
}
