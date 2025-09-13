namespace GameFramework.Resource
{
    public delegate void ClearPackageCacheFilesSuccessCallback(string packageName);
    public delegate void ClearPackageCacheFilesFailureCallback(string packageName, string errorMessage);

    public class ClearPackageCacheFilesCallbacks
    {
        private ClearPackageCacheFilesSuccessCallback m_ClearPackageCacheFilesSuccess;
        private ClearPackageCacheFilesFailureCallback m_ClearPackageCacheFilesFailure;

        public ClearPackageCacheFilesCallbacks(
            ClearPackageCacheFilesSuccessCallback clearPackageCacheFilesSuccess,
            ClearPackageCacheFilesFailureCallback clearPackageCacheFilesFailure)
        {
            m_ClearPackageCacheFilesSuccess = clearPackageCacheFilesSuccess;
            m_ClearPackageCacheFilesFailure = clearPackageCacheFilesFailure;
        }

        public ClearPackageCacheFilesSuccessCallback ClearPackageCacheFilesSuccess => m_ClearPackageCacheFilesSuccess;
        public ClearPackageCacheFilesFailureCallback ClearPackageCacheFilesFailure => m_ClearPackageCacheFilesFailure;
    }
}
