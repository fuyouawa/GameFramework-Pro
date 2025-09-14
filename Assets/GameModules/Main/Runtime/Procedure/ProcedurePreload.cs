using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameFramework.Procedure;
using GameFramework.Resource;
using UnityEngine;
using UnityGameFramework.Runtime;
using YooAsset;
using AssetInfo = GameFramework.Resource.AssetInfo;
using Object = UnityEngine.Object;
using PlayMode = GameFramework.Resource.PlayMode;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace GameMain.Runtime
{
    /// <summary>
    /// 预加载流程
    /// </summary>
    public class ProcedurePreload : ProcedureBase
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

            Log.Debug("Preload assets");
            if (GameConfigAsset.Instance.PreloadAssetTags.Count == 0)
            {
                OnPreloadComplete();
                return;
            }

            var assetInfos = GameEntry.Resource.GetAssetInfos(GameConfigAsset.Instance.PreloadAssetTags.ToArray());

            if (assetInfos.Length > 0)
            {
                UniTask.WhenAll(assetInfos.Select(LoadAssetAsync)).ContinueWith(OnPreloadComplete);
            }
            else
            {
                OnPreloadComplete();
            }
        }

        private async UniTask LoadAssetAsync(AssetInfo assetInfo)
        {
            var asset = await GameEntry.Resource.LoadAssetAsync(assetInfo.AssetName, assetInfo.PackageName, assetInfo.AssetType);
            OnLoadAssetSuccess(asset);
        }

        private void OnLoadAssetSuccess(Object asset)
        {
        }

        private void OnPreloadComplete()
        {
            Log.Info("Preload complete");
            ChangeState<ProcedureLoadAssembly>(_procedureOwner);
        }
    }
}
