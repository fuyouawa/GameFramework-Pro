using UnityEngine.SceneManagement;

namespace GameMain.Runtime
{
    public class LoadSceneParameters
    {
        public LoadSceneMode SceneMode { get; set; } = LoadSceneMode.Single;
        public LocalPhysicsMode PhysicsMode { get; set; } = LocalPhysicsMode.None;
    }
}
