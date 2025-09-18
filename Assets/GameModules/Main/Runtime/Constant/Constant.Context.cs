namespace GameMain.Runtime
{
    public static partial class Constant
    {
        public static class Context
        {
            public static readonly string InitializePackageName = "Context.InitializePackageName";
            public static readonly string PackageDownloader = "Context.PackageDownloader";

            public const string LoadingPhasesCount = "Context.LoadingPhasesCount";
            public static readonly string LoadingPhasesIndex = "Context.LoadingPhasesIndex";

            public static readonly string HookPackageLoadCompleted = "Context.HookPackageLoadCompleted";

            public static readonly string LoadSceneAssetReference = "Context.LoadSceneAssetReference";
        }
    }
}
