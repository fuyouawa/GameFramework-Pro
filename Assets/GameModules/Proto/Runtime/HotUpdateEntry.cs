using System;
using Cysharp.Threading.Tasks;
using GameFramework;
using GameMain.Runtime;
using GameProto.Runtime.Config;
using JetBrains.Annotations;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Runtime.GameEntry;

namespace GameProto.Runtime
{
    [UsedImplicitly]
    class HotUpdateEntry
    {
        [HotUpdateEntry, UsedImplicitly]
        public static async UniTask InitializeAsync()
        {
            await UniTask.WhenAll(Tables.DataTableInfos.Select(LoadTableAsync));
            Log.Info("Load tables complete.");
        }

        private static UniTask LoadTableAsync(Tables.DataTableInfo dataTableInfo)
        {
            return GameEntry.DataTable.LoadDataTableAsync(dataTableInfo.DataRowType,
                Utility.Text.Format(GameConfigAsset.Instance.ConfigAssetName, dataTableInfo.OutputDataFile),
                customPackageName: GameConfigAsset.Instance.ConfigPackageName);
        }
    }
}
