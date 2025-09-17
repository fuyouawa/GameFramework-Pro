using System;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine.SceneManagement;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public class ProcedureStartGame : ProcedureBase
    {
        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            var startSceneAssetReference = GameConfigAsset.Instance.StartSceneAssetReference;
            await GameEntry.Scene.LoadSceneAsync(startSceneAssetReference.AssetName, startSceneAssetReference.PackageName, LoadSceneMode.Additive);
            Log.Debug($"Load start scene success.");
            ChangeState<ProcedureGameing>(procedureOwner);
        }

        protected override string GetLoadingSpinnerDescription(int phaseIndex, int phaseCount)
        {
            return "加载开始界面......";
        }
    }
}
