using GameMain.Runtime;
using UnityGameFramework.Runtime;

namespace GameLogic.Runtime
{
    public class GameApp
    {
        [HotUpdateEntrance]
        public static void Entrance()
        {
            Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
            Log.Warning("======= Entrance GameApp =======");
        }
    }
}
