namespace GameMain.Runtime
{
    public static partial class Constant
    {
        public static class Procedure
        {
            // ProcedureInitPackage => ProcedureUpdateVersion => ProcedureUpdateManifest => ProcedureCreateDownloader =>
            // ProcedureDownloadFiles => ProcedureDownloadOver => ProcedurePreload
            public static readonly int LoadPackagePhaseCount = 7;

            // ProcedureLoadScene => ProcedureInitScene
            public static readonly int LoadScenePhaseCount = 1;

            public static readonly int SplashPhaseCount = LoadPackagePhaseCount + LoadScenePhaseCount + 2;
        }
    }
}
