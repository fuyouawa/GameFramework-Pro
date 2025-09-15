using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        readonly struct AssetInfo
        {
            public readonly string PackageName;
            public readonly string Address;

            public string AssetPath => PackageName + "/" + Address;

            public AssetInfo(string packageName, string address)
            {
                PackageName = packageName;
                Address = address;
            }

            public override int GetHashCode()
            {
                return AssetPath.GetHashCode();
            }

            public override string ToString()
            {
                return AssetPath;
            }
        }

        private static AssetInfo[] s_assetInfosCache;
        private static readonly Dictionary<string, int> AssetInfoIndexByPath = new Dictionary<string, int>();

        private static bool s_refreshing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (s_refreshing || s_assetInfosCache == null)
            {
                EnsureInitialize();
                return EditorGUIUtility.singleLineHeight;
            }

            var assetReference = (AssetReference)property.boxedValue;

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
            if (s_refreshing || s_assetInfosCache == null)
            {
                EnsureInitialize();
                EditorGUI.LabelField(position, label, new GUIContent("Refreshing..."));
                return;
            }

            var assetReference = (AssetReference)property.boxedValue;

            var assetPath = string.IsNullOrEmpty(assetReference.PackageName)
                ? string.Empty
                : $"{assetReference.PackageName}/{assetReference.AssetName}";

            int selectedIndex = -1;
            if (!string.IsNullOrEmpty(assetPath) && !AssetInfoIndexByPath.TryGetValue(assetPath, out selectedIndex))
            {
                MessageBox($"无效资源引用：{assetPath}", MessageType.Error);
                selectedIndex = -1;
            }

            if (string.IsNullOrEmpty(assetPath))
            {
                MessageBox($"资源引用不能为空", MessageType.Error);
            }

            EditorGUI.BeginChangeCheck();
            selectedIndex = EasyEditorGUI.ValueDropdown(position.SubXMax(30), label, selectedIndex, s_assetInfosCache,
                (index, assetInfo) => new GUIContent(assetInfo.AssetPath));

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedIndex != -1)
                {
                    var assetInfo = s_assetInfosCache[selectedIndex];
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

            if (GUI.Button(position.SetXMin(position.xMax - 30),
                    EditorGUIUtility.IconContent("d_Refresh").SetTooltip("刷新缓存")))
            {
                RefreshAssetPaths();
            }

            void MessageBox(string message, MessageType messageType)
            {
                var size = EasyEditorGUI.CalculateMessageBoxSize(message, messageType);
                var rect = position.SetHeight(size.y);
                EasyEditorGUI.MessageBox(rect, message, messageType);
                position.y += size.y;
            }
        }

        private static void EnsureInitialize()
        {
            if (s_assetInfosCache == null)
            {
                RefreshAssetPaths();
            }
        }

        private static void RefreshAssetPaths()
        {
            if (s_refreshing)
                return;

            s_refreshing = true;

            EditorApplication.delayCall += RefreshAssetPathsImpl;
        }

        private static void RefreshAssetPathsImpl()
        {
            var assetInfos = new List<AssetInfo>();
            foreach (var package in AssetBundleCollectorSettingData.Setting.Packages)
            {
                var collectCommand = new CollectCommand(package.PackageName, new NormalIgnoreRule())
                {
                    EnableAddressable = package.EnableAddressable,
                    AutoCollectShaders = package.AutoCollectShaders,
                };

                foreach (var group in package.Groups)
                {
                    foreach (var collector in group.Collectors)
                    {
                        if (collector.IsValid())
                        {
                            foreach (var collectAssetInfo in collector.GetAllCollectAssets(collectCommand, group))
                            {
                                var assetInfo = new AssetInfo(package.PackageName, collectAssetInfo.Address);
                                assetInfos.Add(assetInfo);
                            }
                        }
                    }
                }
            }

            s_assetInfosCache = assetInfos.Distinct().ToArray();

            AssetInfoIndexByPath.Clear();
            for (var i = 0; i < s_assetInfosCache.Length; i++)
            {
                AssetInfoIndexByPath.Add(s_assetInfosCache[i].AssetPath, i);
            }

            s_refreshing = false;
        }
    }
}
