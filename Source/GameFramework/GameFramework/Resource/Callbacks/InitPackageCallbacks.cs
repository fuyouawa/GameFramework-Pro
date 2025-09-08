namespace GameFramework.Resource
{
    public delegate void InitPackageSuccessCallback(string packageName);
    public delegate void InitPackageFailureCallback(string packageName, string errorMessage, object userData);

    public class InitPackageCallbacks
    {
        private readonly InitPackageSuccessCallback m_InitPackageSuccess;
        private readonly InitPackageFailureCallback m_InitPackageFailure;

        public InitPackageCallbacks(InitPackageSuccessCallback initPackageSuccessCallback,
            InitPackageFailureCallback initPackageFailureCallback)
        {
            m_InitPackageSuccess = initPackageSuccessCallback;
            m_InitPackageFailure = initPackageFailureCallback;
        }

        public InitPackageSuccessCallback InitPackageSuccess => m_InitPackageSuccess;
        public InitPackageFailureCallback InitPackageFailure => m_InitPackageFailure;
    }

}
