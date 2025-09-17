using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public class ProcedureLoadScene : ProcedureBase
    {
        enum LoadSceneState
        {
            LoadingNewScene,
            UnloadingPreviousScene,
            LoadingComplete,
        }

        private AssetReference _previousSceneAssetReference;
        private LoadSceneState _loadSceneState;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            var loadSceneAssetReference = GameEntry.Context.Get<AssetReference>(Constant.Context.LoadSceneAssetReference);

            _loadSceneState = LoadSceneState.LoadingNewScene;

            if (!await LoadSceneWithRetryAsync(loadSceneAssetReference))
            {
                ChangeState<ProcedureFatalError>(procedureOwner);
                return;
            }

            if (_previousSceneAssetReference != null)
            {
                _loadSceneState = LoadSceneState.UnloadingPreviousScene;
                if (!await UnloadSceneWithRetryAsync(_previousSceneAssetReference))
                {
                    ChangeState<ProcedureFatalError>(procedureOwner);
                    return;
                }
            }
            _loadSceneState = LoadSceneState.LoadingComplete;
            _previousSceneAssetReference = loadSceneAssetReference;
            ChangeState<ProcedureGameing>(procedureOwner);
        }


        private async UniTask<bool> LoadSceneWithRetryAsync(AssetReference loadSceneAssetReference, int retryCount = 0)
        {
            try
            {
                var scene = await GameEntry.Scene.LoadSceneAsync(loadSceneAssetReference.AssetName, loadSceneAssetReference.PackageName, LoadSceneMode.Additive);
                Log.Debug($"Load scene '{loadSceneAssetReference.AssetName}' success.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Load scene '{loadSceneAssetReference.AssetName}' failed: {e}");
                var index = await GameEntry.UI.ShowMessageBoxAsync($"加载场景“{loadSceneAssetReference.AssetName}”失败，是否尝试重新加载？",
                    UIMessageBoxType.Error,
                    UIMessageBoxButtons.YesNo);
                if (index == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数。", UIMessageBoxType.Fatal);
                        return false;
                    }

                    if (await LoadSceneWithRetryAsync(loadSceneAssetReference, retryCount + 1))
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private async UniTask<bool> UnloadSceneWithRetryAsync(AssetReference loadSceneAssetReference, int retryCount = 0)
        {
            try
            {
                await GameEntry.Scene.UnloadSceneAsync(loadSceneAssetReference.AssetName, loadSceneAssetReference.PackageName);
                Log.Debug($"Unload scene '{loadSceneAssetReference.AssetName}' success.");
                return true;
            }
            catch (Exception e)
            {
                Log.Error($"Unload scene '{loadSceneAssetReference.AssetName}' failed: {e}");
                var index = await GameEntry.UI.ShowMessageBoxAsync($"卸载场景“{loadSceneAssetReference.AssetName}”失败，是否尝试重新卸载？", UIMessageBoxType.Error, UIMessageBoxButtons.YesNo);
                if (index == 0)
                {
                    if (retryCount >= GameEntry.Resource.FailedTryAgain)
                    {
                        await GameEntry.UI.ShowMessageBoxAsync($"已重试达到最大次数。", UIMessageBoxType.Fatal);
                        return false;
                    }
                }

                if (await UnloadSceneWithRetryAsync(loadSceneAssetReference, retryCount + 1))
                {
                    return true;
                }
                return false;
            }
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return _loadSceneState switch
            {
                LoadSceneState.LoadingNewScene => "加载新场景......",
                LoadSceneState.UnloadingPreviousScene => "卸载旧场景......",
                _ => throw new Exception("Invalid load scene state."),
            };
        }
    }
}