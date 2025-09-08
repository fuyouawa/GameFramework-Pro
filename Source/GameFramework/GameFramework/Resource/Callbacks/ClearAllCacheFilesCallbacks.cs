namespace GameFramework.Resource
{
    public delegate void ClearAllUnusedCacheFilesCompleteCallback(bool hasError);

    public class ClearAllCacheFilesCallbacks
    {
        private ClearAllUnusedCacheFilesCompleteCallback m_ClearAllUnusedCacheFilesComplete;
        private ClearPackageUnusedCacheFilesSuccessCallback m_ClearPackageUnusedCacheFilesSuccess;
        private ClearPackageUnusedCacheFilesFailureCallback m_ClearPackageUnusedCacheFilesFailure;

        public ClearAllCacheFilesCallbacks(
            ClearAllUnusedCacheFilesCompleteCallback clearAllUnusedCacheFilesComplete,
            ClearPackageUnusedCacheFilesSuccessCallback clearPackageUnusedCacheFilesSuccess,
            ClearPackageUnusedCacheFilesFailureCallback clearPackageUnusedCacheFilesFailure)
        {
            m_ClearAllUnusedCacheFilesComplete = clearAllUnusedCacheFilesComplete;
            m_ClearPackageUnusedCacheFilesSuccess = clearPackageUnusedCacheFilesSuccess;
            m_ClearPackageUnusedCacheFilesFailure = clearPackageUnusedCacheFilesFailure;
        }

        public ClearAllUnusedCacheFilesCompleteCallback ClearAllUnusedCacheFilesComplete =>
            m_ClearAllUnusedCacheFilesComplete;

        public ClearPackageUnusedCacheFilesSuccessCallback ClearPackageUnusedCacheFilesSuccess =>
            m_ClearPackageUnusedCacheFilesSuccess;

        public ClearPackageUnusedCacheFilesFailureCallback ClearPackageUnusedCacheFilesFailure =>
            m_ClearPackageUnusedCacheFilesFailure;
    }
}
