using System.Collections.Generic;
using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;

namespace GameMain.Runtime
{
    [EasyInspector]
    [ScriptableObjectSingletonAssetPath("Resources/Settings")]
    public class GameSettings : ScriptableObjectSingleton<GameSettings>
    {
        [Title("Assets")]
        [SerializeField] private List<string> _preloadAssetTags;

        [Title("HybridCLR")]
        [SerializeField] private List<string> _hotUpdateAssemblies = new List<string>();
        [SerializeField] private List<string> _aotMetaAssemblies = new List<string>();
        [SerializeField] private string _hotUpdateEntryDllName;

        public IReadOnlyList<string> PreloadAssetTags => _preloadAssetTags;

        public IReadOnlyList<string> HotUpdateAssemblies => _hotUpdateAssemblies;
        public IReadOnlyList<string> AOTMetaAssemblies => _aotMetaAssemblies;
        public string HotUpdateEntryDllName => _hotUpdateEntryDllName;
    }
}
