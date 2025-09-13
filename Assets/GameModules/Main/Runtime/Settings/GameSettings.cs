using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using UnityEngine;

namespace GameMain.Runtime
{
    [ScriptableObjectSingletonAssetPath("Assets/Resources")]
    public class GameSettings : ScriptableObjectSingleton<GameSettings>
    {
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
