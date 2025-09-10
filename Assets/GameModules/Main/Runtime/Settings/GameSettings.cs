using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameMain.Runtime
{
    public class GameSettings : ScriptableObject
    {
        private static GameSettings s_instance;

        public static GameSettings Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = Resources.Load<GameSettings>("GameSettings");
                    if (s_instance == null)
                    {
#if UNITY_EDITOR
                        s_instance = CreateInstance<GameSettings>();
                        UnityEditor.AssetDatabase.CreateAsset(s_instance, "Assets/Resources/GameSettings.asset");
#else
                        throw new Exception("GameSettings is not found.");
#endif
                    }
                }
                return s_instance;
            }
        }

        [Header("Assets")]
        [SerializeField] private List<string> _preloadAssetTags;

        [Header("HybridCLR")]
        [SerializeField] private List<string> _hotUpdateAssemblyNames = new List<string>();

        [SerializeField] private List<string> _aotMetaAssemblyNames = new List<string>();

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public IReadOnlyList<string> HotUpdateAssemblyNames => _hotUpdateAssemblyNames;
        public IReadOnlyList<string> AOTMetaAssemblyNames => _aotMetaAssemblyNames;

        public Comparison<Assembly> HotUpdateAssemblyComparison => (x, y) =>
        {
            var i = _hotUpdateAssemblyNames.IndexOf(x.GetName().Name);
            var j = _hotUpdateAssemblyNames.IndexOf(y.GetName().Name);
            return i.CompareTo(j);
        };
    }
}
