using System;
using Cysharp.Threading.Tasks;
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
        public static void Initialize()
        {
            UniTask.WhenAll(Tables.DataTableInfos.Select(LoadTableAsync)).ContinueWith(() =>
            {
                Log.Info("Load tables success.");
            }).Forget();
        }

        private static UniTask LoadTableAsync(Tables.DataTableInfo dataTableInfo)
        {
            return GameEntry.DataTable.LoadDataTableAsync(dataTableInfo.DataRowType, dataTableInfo.OutputDataFile);
        }
    }
}
