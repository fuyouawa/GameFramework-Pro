namespace GameFramework.Resource
{
    public delegate void ClearPackageUnusedCacheFilesSuccessCallback(string packageName);
    public delegate void ClearPackageUnusedCacheFilesFailureCallback(string packageName, string errorMessage);

    public class ClearPackageCacheFilesCallbacks
    {
        private ClearPackageUnusedCacheFilesSuccessCallback m_ClearPackageUnusedCacheFilesSuccess;
        private ClearPackageUnusedCacheFilesFailureCallback m_ClearPackageUnusedCacheFilesFailure;

        public ClearPackageCacheFilesCallbacks(
            ClearPackageUnusedCacheFilesSuccessCallback clearPackageUnusedCacheFilesSuccess,
            ClearPackageUnusedCacheFilesFailureCallback clearPackageUnusedCacheFilesFailure)
        {
            m_ClearPackageUnusedCacheFilesSuccess = clearPackageUnusedCacheFilesSuccess;
            m_ClearPackageUnusedCacheFilesFailure = clearPackageUnusedCacheFilesFailure;
        }

        public ClearPackageUnusedCacheFilesSuccessCallback ClearPackageUnusedCacheFilesSuccess => m_ClearPackageUnusedCacheFilesSuccess;
        public ClearPackageUnusedCacheFilesFailureCallback ClearPackageUnusedCacheFilesFailure => m_ClearPackageUnusedCacheFilesFailure;
    }
}
