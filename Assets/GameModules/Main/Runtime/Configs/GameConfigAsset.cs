using System;
using System.Collections.Generic;
using System.Reflection;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;

namespace GameMain.Runtime
{
    [EasyInspector]
    [ScriptableObjectSingletonAssetPath("Assets/Resources/Configs")]
    public class GameConfigAsset : ScriptableObjectSingleton<GameConfigAsset>
    {
        [Title("Assets")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _preloadAssetTags;

        [Title("Proto")]
        [SerializeField] private string _configAssetName = "Config_{0}";

        [Title("HybridCLR")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _hotUpdateAssemblyNames = new List<string>();

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _aotMetaAssemblyNames = new List<string>();

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public string ConfigAssetName => _configAssetName;

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
