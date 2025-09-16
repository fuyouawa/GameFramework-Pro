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

        protected override async UniTask OnEnterAsync(ProcedureOwner procedureOwner)
        {
            var packageName = GameEntry.Context.Get<string>(Constant.Context.InitializePackageName);

            GameEntry.Context.Set(Constant.Context.InitializePackageName, packageName);
            if (await InitializePackageWithRetryAsync(packageName))
            {
                if (packageName == GameEntry.Resource.DefaultPackageName)
                {
                    YooAssets.SetDefaultPackage(YooAssetsHelper.GetPackage(GameEntry.Resource.DefaultPackageName));
                }

                ChangeState<ProcedureUpdateVersion>(_procedureOwner);
            }
            else
            {
                ChangeState<ProcedureEndGame>(_procedureOwner);
            }
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            var packageName = GameEntry.Context.Get<string>(Constant.Context.InitializePackageName);
            return $"初始化资源包“{packageName}”......";
        }

        private async UniTask<bool> InitializePackageWithRetryAsync(string packageName, int retryCount = 0)
        {
            try
            {
                await InitializePackageAsync(packageName, GameEntry.Resource.PlayMode);
                Log.Debug($"Initialize default package '{packageName}' success.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Initialize default package '{packageName}' failed: {e}");

                var result = await GameEntry.UI.ShowMessageBoxAsync($"初始化资源包“{packageName}”失败，是否尝试重新初始化",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (result == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数，游戏即将退出。", UIMessageBoxType.Error);
                        return false;
                    }

                    if (await InitializePackageWithRetryAsync(packageName, retryCount + 1))
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        public static async UniTask InitializePackageAsync(string packageName, PlayMode playMode)
        {
            var package = YooAssets.TryGetPackage(packageName);
            if (package is { InitializeStatus: EOperationStatus.Succeed })
            {
                return;
            }

            package = YooAssets.CreatePackage(packageName);

            // 编辑器下的模拟模式
            InitializationOperation initializationOperation = null;
            switch (playMode)
            {
                case PlayMode.EditorSimulateMode:
                {
                    var buildResult = EditorSimulateModeHelper.SimulateBuild(packageName);
                    var packageRoot = buildResult.PackageRootDirectory;
                    var editorFileSystemParams =
                        FileSystemParameters.CreateDefaultEditorFileSystemParameters(packageRoot);
                    var initParameters = new EditorSimulateModeParameters();
                    initParameters.EditorFileSystemParameters = editorFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // 单机运行模式
                case PlayMode.OfflinePlayMode:
                {
                    var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();
                    var initParameters = new OfflinePlayModeParameters();
                    initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // 联机运行模式
                case PlayMode.HostPlayMode:
                {
                    IRemoteServices remoteServices = new RemoteServices(GameEntry.Resource.HostServerURL,
                        GameEntry.Resource.FallbackHostServerURL);
                    var cacheFileSystemParams =
                        FileSystemParameters.CreateDefaultCacheFileSystemParameters(remoteServices);
                    var buildinFileSystemParams = FileSystemParameters.CreateDefaultBuildinFileSystemParameters();

                    var initParameters = new HostPlayModeParameters();
                    initParameters.BuildinFileSystemParameters = buildinFileSystemParams;
                    initParameters.CacheFileSystemParameters = cacheFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                // WebGL运行模式
                case PlayMode.WebPlayMode:
                {
                    IRemoteServices remoteServices = new RemoteServices(GameEntry.Resource.HostServerURL,
                        GameEntry.Resource.FallbackHostServerURL);
                    var webServerFileSystemParams = FileSystemParameters.CreateDefaultWebServerFileSystemParameters();
                    var webRemoteFileSystemParams =
                        FileSystemParameters.CreateDefaultWebRemoteFileSystemParameters(remoteServices); //支持跨域下载

                    var initParameters = new WebPlayModeParameters();
                    initParameters.WebServerFileSystemParameters = webServerFileSystemParams;
                    initParameters.WebRemoteFileSystemParameters = webRemoteFileSystemParams;
                    initializationOperation = package.InitializeAsync(initParameters);
                    break;
                }
                default:
                    throw new InvalidOperationException();
            }

            await initializationOperation.ToUniTask();
        }
    }
}
