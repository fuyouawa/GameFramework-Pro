using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;

namespace GameMain.Runtime
{
    [EasyInspector]
    [ScriptableObjectSingletonAssetPath("Assets/Resources/Settings")]
    public class GameSettings : ScriptableObjectSingleton<GameSettings>
    {
        [Title("Assets")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _preloadAssetTags;

        [Title("HybridCLR")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _hotUpdateAssemblyNames = new List<string>();

        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<string> _aotMetaAssemblyNames = new List<string>();

        [SerializeField] private string _hotUpdateEntryAssemblyName;

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public IReadOnlyList<string> HotUpdateAssemblyNames => _hotUpdateAssemblyNames;
        public IReadOnlyList<string> AOTMetaAssemblyNames => _aotMetaAssemblyNames;
        public string HotUpdateEntryAssemblyName => _hotUpdateEntryAssemblyName;
    }
}
