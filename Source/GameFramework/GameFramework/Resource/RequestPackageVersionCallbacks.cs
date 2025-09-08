namespace GameFramework.Resource
{
    public delegate void RequestPackageVersionSuccessCallback(string packageName, string packageVersion);
    public delegate void RequestPackageVersionFailureCallback(string packageName, string errorMessage);

    public class RequestPackageVersionCallbacks
    {
        private RequestPackageVersionSuccessCallback m_RequestPackageVersionSuccessCallback;
        private RequestPackageVersionFailureCallback m_RequestPackageVersionFailureCallback;

        public RequestPackageVersionCallbacks(RequestPackageVersionSuccessCallback requestPackageVersionSuccessCallback,
            RequestPackageVersionFailureCallback requestPackageVersionFailureCallback)
        {
            m_RequestPackageVersionSuccessCallback = requestPackageVersionSuccessCallback;
            m_RequestPackageVersionFailureCallback = requestPackageVersionFailureCallback;
        }

        public RequestPackageVersionSuccessCallback RequestPackageVersionSuccessCallback => m_RequestPackageVersionSuccessCallback;
        public RequestPackageVersionFailureCallback RequestPackageVersionFailureCallback => m_RequestPackageVersionFailureCallback;
    }
}
