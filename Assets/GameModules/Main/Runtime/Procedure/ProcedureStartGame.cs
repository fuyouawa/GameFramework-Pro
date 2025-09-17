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
        protected override Func<int, int, string> LoadingSpinnerDescriptionGetter => null;

        protected override async UniTask OnEnterAsync(IFsm<IProcedureManager> procedureOwner)
        {
            var startSceneAssetReference = GameConfigAsset.Instance.StartSceneAssetReference;
            GameEntry.Context.Set(Constant.Context.LoadSceneAssetReference, startSceneAssetReference);
            ChangeState<ProcedureLoadScene>(procedureOwner);
        }
    }
}
