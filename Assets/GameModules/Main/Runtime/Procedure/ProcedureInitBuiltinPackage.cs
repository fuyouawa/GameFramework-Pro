﻿using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityGameFramework.Runtime;
using YooAsset;

namespace GameMain.Runtime
{
    public class ProcedureInitBuiltinPackage : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            YooAssets.Initialize();

            try
            {
                GameEntry.Context.Set(Constant.Context.InitializePackageName, Constant.Package.Builtin);

#if UNITY_EDITOR
                await ProcedureInitPackage.InitializePackageAsync(Constant.Package.Builtin, PlayMode.EditorSimulateMode);
#else
                await ProcedureInitPackage.InitializePackageAsync(Constant.Package.Builtin, PlayMode.OfflinePlayMode);
#endif

                var package = YooAssetsHelper.GetPackage(Constant.Package.Builtin);

                var packageVersionOperation = package.RequestPackageVersionAsync();
                await packageVersionOperation.ToUniTask();
                var packageVersion = packageVersionOperation.PackageVersion;

                GameEntry.Setting.SetString(Utility.Text.Format(Constant.Setting.PackageVersion, Constant.Package.Builtin), packageVersion);

                var packageManifestOperation = package.UpdatePackageManifestAsync(packageVersion);
                await packageManifestOperation.ToUniTask();

                Log.Debug($"Initialize builtin package success.");

                ChangeState<ProcedureSplash>(procedureOwner);
            }
            catch (Exception e)
            {
                Log.Error($"Initialize builtin package failed: {e}");
                SafeErrorBox.Show("初始化内置资源包失败，游戏即将退出。");
                ChangeState<ProcedureEndGame>(procedureOwner);
            }
        }
    }
}
