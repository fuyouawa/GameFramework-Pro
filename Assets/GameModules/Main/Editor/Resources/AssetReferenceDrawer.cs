using System.Collections.Generic;
using System.Linq;
using EasyToolKit.Core;
using EasyToolKit.Core.Editor;
using GameMain.Runtime;
using UnityEditor;
using UnityEngine;
using YooAsset;
using YooAsset.Editor;

namespace GameMain.Editor
{
    [CustomPropertyDrawer(typeof(AssetReference))]
    public class AssetReferenceDrawer : PropertyDrawer
    {
        struct AssetInfo
        {
            public readonly string PackageName;
            public readonly string Address;

            public string AssetPath => PackageName + "/" + Address;

            public AssetInfo(string packageName, string address)
            {
                PackageName = packageName;
                Address = address;
            }
        }

        private static readonly List<AssetInfo> AssetInfosCache = new List<AssetInfo>();
        private static readonly Dictionary<string, int> AssetInfoIndexByPath = new Dictionary<string, int>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var assetReference = (AssetReference)property.boxedValue;
            EnsureInitialize();

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";
            if (string.IsNullOrEmpty(assetPath) || AssetInfoIndexByPath.ContainsKey(assetPath))
            {
                return EditorGUIUtility.singleLineHeight;
            }
            else
            {
                var size = EasyEditorGUI.CalculateMessageBoxSize($"无效资源引用：{assetPath}", MessageType.Error);
                return EditorGUIUtility.singleLineHeight + size.y;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var assetReference = (AssetReference)property.boxedValue;
            EnsureInitialize();

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";

            int selectedIndex = -1;
            if (!string.IsNullOrEmpty(assetPath) && !AssetInfoIndexByPath.TryGetValue(assetPath, out selectedIndex))
            {
                MessageBox($"无效资源引用：{assetPath}", MessageType.Error);
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                MessageBox($"资源引用不能为空", MessageType.Error);
            }

            EditorGUI.BeginChangeCheck();
            selectedIndex = EasyEditorGUI.ValueDropdown(position, label, selectedIndex, AssetInfosCache,
                (index, assetInfo) => new GUIContent(assetInfo.AssetPath));

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex != -1)
                {
                    var assetInfo = AssetInfosCache[selectedIndex];
                    assetReference.PackageName = assetInfo.PackageName;
                    assetReference.AssetName = assetInfo.Address;
                }
                else
                {
                    assetReference.PackageName = string.Empty;
                    assetReference.AssetName = string.Empty;
                }
                property.boxedValue = assetReference;
            }

            void MessageBox(string message, MessageType messageType)
            {
                var size = EasyEditorGUI.CalculateMessageBoxSize(message, MessageType.Error);
                var rect = position.SetHeight(size.y);
                EasyEditorGUI.MessageBox(rect, message, MessageType.Error);
                position.y += size.y;
            }
        }

        private static void EnsureInitialize()
        {
            if (AssetInfosCache.Count == 0)
            {
                RefreshAssetPaths();
            }
        }

        private static void RefreshAssetPaths()
        {
            AssetInfosCache.Clear();
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                var collectCommand = new CollectCommand(package.PackageName, new NormalIgnoreRule())
                {
                    EnableAddressable = package.EnableAddressable,
                    AutoCollectShaders = package.AutoCollectShaders
                };

                foreach (var group in package.Groups)
                {
                    foreach (var collector in group.Collectors)
                    {
                        if (collector.IsValid())
                        {
                            foreach (var assetInfo in collector.GetAllCollectAssets(collectCommand, group))
                            {
                                AssetInfosCache.Add(new AssetInfo(package.PackageName, assetInfo.Address));
                            }
                        }
                    }
                }
            }

            AssetInfoIndexByPath.Clear();
            for (var i = 0; i < AssetInfosCache.Count; i++)
            {
                AssetInfoIndexByPath.Add(AssetInfosCache[i].AssetPath, i);
            }
        }
    }
}
