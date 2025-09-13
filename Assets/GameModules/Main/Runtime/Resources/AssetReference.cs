using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GameMain.Runtime
{
    [Serializable]
    public class AssetReference
    {
        [SerializeField] private string _packageName;
        [SerializeField] private string _assetName;

        public string PackageName
        {
            get => _packageName;
            set => _packageName = value;
        }

        public string AssetName
        {
            get => _assetName;
            set => _assetName = value;
        }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(_packageName) && !string.IsNullOrEmpty(_assetName);
        }

        public UniTask<UnityEngine.Object> LoadAssetAsync()
        {
            return GameEntry.Resource.LoadAssetAsync(_assetName, _packageName);
        }

        public UniTask<T> LoadAssetAsync<T>()
            where T : UnityEngine.Object
        {
            return GameEntry.Resource.LoadAssetAsync<T>(_assetName, _packageName);
        }
    }
}
