using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    /// <summary>
    /// 游戏入口。
    /// </summary>
    public partial class GameEntry : MonoBehaviour
    {
        public static ContextComponent Context { get; private set; }

        private static void InitCustomComponents()
        {
            Context = UnityGameFramework.Runtime.GameEntry.GetComponent<ContextComponent>();
        }
    }
}
