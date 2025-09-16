using System;
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

        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            var tasks = YooAssets.GetAllPackages()
                .Select(package => ClearCacheFilesAsync(package, EFileClearMode.ClearUnusedBundleFiles));
            await UniTask.WhenAll(tasks);
            Log.Debug("Clear cache files complete.");
        }

        private static async UniTask ClearCacheFilesAsync(ResourcePackage package, EFileClearMode clearMode)
        {
            try
            {
                var operation = package.ClearCacheFilesAsync(clearMode);
                await operation.ToUniTask();
            }
            catch (Exception e)
            {
                Log.Error($"Clear cache files by mode '{clearMode}' failed: {e}");
            }
        }
    }
}
