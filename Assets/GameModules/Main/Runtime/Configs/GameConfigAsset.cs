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
        [SerializeField] private string _configPackageName = "Common";
        [SerializeField] private string _configAssetName = "Config_{0}";

        [Title("HybridCLR")]
        [SerializeField] private string _assemblyPackageName = "Common";
        [SerializeField] private string _assemblyAssetName = "DLL_{0}";

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _hotUpdateAssemblyNames = new List<string>();

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _aotMetaAssemblyNames = new List<string>();

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public string ConfigPackageName => _configPackageName;
        public string ConfigAssetName => _configAssetName;

        public string AssemblyPackageName => _assemblyPackageName;
        public string AssemblyAssetName => _assemblyAssetName;

        public IReadOnlyList<string> HotUpdateAssemblyNames => _hotUpdateAssemblyNames;
        public IReadOnlyList<string> AOTMetaAssemblyNames => _aotMetaAssemblyNames;
    }
}
