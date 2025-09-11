using GameMain.Runtime;
using GameProto.Runtime;
using JetBrains.Annotations;
using UnityGameFramework.Runtime;
using GameEntry = GameMain.Runtime.GameEntry;

namespace GameLogic.Runtime
{
    [UsedImplicitly]
    public class HotUpdateEntry
    {
        [HotUpdateEntry, UsedImplicitly]
        public static void Initialize()
        {
            Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
            Log.Warning("======= Entrance =======");

            // GameEntry.DataTable.GetDataTable<Test>();
        }
    }
}
