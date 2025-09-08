namespace GameFramework.Resource
{
    public delegate void ClearAllCacheFilesCompleteCallback(bool hasError);

    public class ClearAllCacheFilesCallbacks
    {
        private ClearAllCacheFilesCompleteCallback m_ClearAllCacheFilesComplete;
        private ClearPackageCacheFilesSuccessCallback m_ClearPackageCacheFilesSuccess;
        private ClearPackageCacheFilesFailureCallback m_ClearPackageCacheFilesFailure;

        public ClearAllCacheFilesCallbacks(
            ClearAllCacheFilesCompleteCallback clearAllCacheFilesComplete,
            ClearPackageCacheFilesSuccessCallback clearPackageCacheFilesSuccess,
            ClearPackageCacheFilesFailureCallback clearPackageCacheFilesFailure)
        {
            m_ClearAllCacheFilesComplete = clearAllCacheFilesComplete;
            m_ClearPackageCacheFilesSuccess = clearPackageCacheFilesSuccess;
            m_ClearPackageCacheFilesFailure = clearPackageCacheFilesFailure;
        }

        public ClearAllCacheFilesCompleteCallback ClearAllCacheFilesComplete =>
            m_ClearAllCacheFilesComplete;

        public ClearPackageCacheFilesSuccessCallback ClearPackageCacheFilesSuccess =>
            m_ClearPackageCacheFilesSuccess;

        public ClearPackageCacheFilesFailureCallback ClearPackageCacheFilesFailure =>
            m_ClearPackageCacheFilesFailure;
    }
}
