namespace GameFramework.Resource
{
    public delegate void UpdatePackageManifestSuccessCallback(string packageName);

    public delegate void UpdatePackageManifestFailureCallback(string packageName, string errorMessage);

    public class UpdatePackageManifestCallbacks
    {
        private UpdatePackageManifestSuccessCallback m_UpdatePackageManifestSuccessCallback;
        private UpdatePackageManifestFailureCallback m_UpdatePackageManifestFailureCallback;

        public UpdatePackageManifestCallbacks(UpdatePackageManifestSuccessCallback updatePackageManifestSuccessCallback,
            UpdatePackageManifestFailureCallback updatePackageManifestFailureCallback)
        {
            m_UpdatePackageManifestSuccessCallback = updatePackageManifestSuccessCallback;
            m_UpdatePackageManifestFailureCallback = updatePackageManifestFailureCallback;
        }

        public UpdatePackageManifestSuccessCallback UpdatePackageManifestSuccessCallback =>
            m_UpdatePackageManifestSuccessCallback;

        public UpdatePackageManifestFailureCallback UpdatePackageManifestFailureCallback =>
            m_UpdatePackageManifestFailureCallback;
    }
}
