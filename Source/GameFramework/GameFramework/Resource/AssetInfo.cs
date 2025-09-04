using System;

namespace GameFramework.Resource
{
    public class AssetInfo
    {
        private readonly string m_PackageName;
        private readonly Type m_AssetType;
        private readonly string m_Error;
        private readonly string m_AssetName;
        private readonly string m_AssetPath;

        private readonly object m_Reference;


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
        /// 资源路径
        /// </summary>
        public string AssetPath => m_AssetPath;

        /// <summary>
        /// 引用对象
        /// </summary>
        public object Reference => m_Reference;

        public AssetInfo(string packageName, Type assetType, string assetName, string assetPath, string error, object reference)
        {
            m_PackageName = packageName;
            m_AssetType = assetType;
            m_AssetName = assetName;
            m_AssetPath = assetPath;
            m_Error = error;
            m_Reference = reference;
        }
    }
}
