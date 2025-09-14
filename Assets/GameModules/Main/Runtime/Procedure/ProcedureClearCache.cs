using Cysharp.Threading.Tasks;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 清理缓存。
    /// </summary>
    public class ProcedureClearCache : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            _procedureOwner = procedureOwner;
            Log.Debug("清理未使用的缓存文件！");

            // UILoadMgr.Show(UIDefine.UILoadUpdate,$"清理未使用的缓存文件...");
            //
            // var operation = GameModule.Resource.ClearUnusedCacheFilesAsync();
            // operation.Completed += Operation_Completed;

        }
    }
}
