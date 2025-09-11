using System;

namespace GameFramework.Resource
{
    public class AssetInfo
    {
        private readonly string m_PackageName;
        private readonly Type m_AssetType;
        private readonly string m_Error;
        private readonly string m_AssetName;

        private readonly object m_UserData;


        /// <summary>
        /// 所属包裹
        /// </summary>
        public string PackageName => m_PackageName;

        /// <summary>
        /// 资源类型
        /// </summary>
        public Type AssetType => m_AssetType;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error => m_Error;

        /// <summary>
        /// 资源名称
        /// </summary>
        public string AssetName => m_AssetName;

        /// <summary>
        /// 引用对象
        /// </summary>
        public object UserData => m_UserData;

        public AssetInfo(string packageName, Type assetType, string assetName, string error, object userData)
        {
            m_PackageName = packageName;
            m_AssetType = assetType;
            m_AssetName = assetName;
            m_Error = error;
            m_UserData = userData;
        }
    }
}
