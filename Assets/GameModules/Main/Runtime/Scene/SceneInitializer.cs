using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain.Runtime
{
    public class SceneInitializer : MonoBehaviour
    {
        public UniTask InitializeAsync()
        {
            return OnInitializeAsync();
        }

        protected virtual UniTask OnInitializeAsync()
        {
            return UniTask.CompletedTask;
        }
    }
}