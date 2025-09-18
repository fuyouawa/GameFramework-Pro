namespace GameMain.Runtime
{
    public static partial class Constant
    {
        public static class Procedure
        {
            public const int SplashPhaseCount = LoadPackagePhaseCount + LoadScenePhaseCount + 2;

            // ProcedureInitPackage => ProcedureUpdateVersion => ProcedureUpdateManifest => ProcedureCreateDownloader =>
            // ProcedureDownloadFiles => ProcedureDownloadOver => ProcedurePreload
            public const int LoadPackagePhaseCount = 7;

            // ProcedureLoadScene => ProcedureInitScene
            public const int LoadScenePhaseCount = 1;
        }
    }
}
