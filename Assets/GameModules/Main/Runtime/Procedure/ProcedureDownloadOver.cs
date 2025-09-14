using GameFramework.Procedure;
using UnityGameFramework.Runtime;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    public class ProcedureDownloadOver : ProcedureBase
    {
        private bool _needClearCache;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            Log.Debug("DownloadOver");
            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"下载完成...");
        }

        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            ChangeState<ProcedurePreload>(procedureOwner);
        }
    }
}
