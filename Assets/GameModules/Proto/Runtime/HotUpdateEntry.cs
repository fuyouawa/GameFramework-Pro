using System;
using Cysharp.Threading.Tasks;
using GameMain.Runtime;
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
            TablesLoader.LoadTablesAsync(LoadTableAsync).ContinueWith(() =>
            {
                Log.Debug("Load tables success.");
            }).Forget();
        }

        private static UniTask LoadTableAsync(Type tableType, string tableName)
        {
            return GameEntry.DataTable.LoadDataTableAsync(tableType, tableName);
        }
    }
}
