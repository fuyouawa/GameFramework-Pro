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
    /// 流程 => 初始化Package。
    /// </summary>
    public class ProcedureInitPackage : ProcedureBase
    {
        private ProcedureOwner _procedureOwner;

        protected override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);

            _procedureOwner = procedureOwner;
        }

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);

            YooAssets.Initialize();

            InitPackagesAsync().Forget();
        }

        private async UniTask InitPackagesAsync()
        {
            try
            {
                await GameEntry.Resource.InitializePackageAsync(Constant.Package.Builtin,
                    GameEntry.Resource.PlayMode == PlayMode.EditorSimulateMode
                        ? PlayMode.EditorSimulateMode
                        : PlayMode.OfflinePlayMode);
                Log.Debug($"Initialize builtin package success.");
            }
            catch (Exception e)
            {
                Log.Error($"Initialize builtin package failed: {e}");
                SafeErrorBox.Show("初始化内置资源包失败，游戏即将退出。");
                ChangeState<ProcedureEndGame>(_procedureOwner);
            }

            await InitializeDefaultPackageAsync();
        }

        private async UniTask InitializeDefaultPackageAsync()
        {
            try
            {
                await GameEntry.Resource.InitializePackageAsync(GameEntry.Resource.DefaultPackageName,
                    GameEntry.Resource.PlayMode);
                Log.Debug($"Initialize default package '{GameEntry.Resource.DefaultPackageName}' success.");
                ChangeState<ProcedureUpdateVersion>(_procedureOwner);
            }
            catch (Exception e)
            {
                Log.Error($"Initialize default package '{GameEntry.Resource.DefaultPackageName}' failed: {e}");

                var result = await GameEntry.UI.ShowMessageBoxAsync("初始化资源包失败，是否尝试重新初始化", UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    InitializeDefaultPackageAsync().Forget();
                }
                else
                {
                    ChangeState<ProcedureEndGame>(_procedureOwner);
                }
            }
        }
    }
}
