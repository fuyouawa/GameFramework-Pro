//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using System;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace UnityGameFramework.Editor
{
    [CustomEditor(typeof(ResourceComponent))]
    internal sealed class ResourceComponentInspector : GameFrameworkInspector
    {
        private static readonly string[] _resourceModeNames = new string[]
        {
            "EditorSimulateMode (编辑器下的模拟模式)",
            "OfflinePlayMode (单机模式)",
            "HostPlayMode (联机运行模式)",
            "WebPlayMode (WebGL运行模式)"
        };

        private static readonly string[] _verifyLevelNames = new string[]
        {
            "Low (验证文件存在)",
            "Middle (验证文件大小)",
            "High (验证文件大小和CRC)"
        };

        private SerializedProperty m_PlayMode = null;
        private SerializedProperty m_DefaultPackageName = null;
        // private SerializedProperty m_UpdatableWhilePlaying = null;
        private SerializedProperty m_FileVerifyLevel = null;
        private SerializedProperty m_Milliseconds = null;
        // private SerializedProperty m_ReadWritePathType = null;
        private SerializedProperty m_MinUnloadUnusedAssetsInterval = null;
        private SerializedProperty m_MaxUnloadUnusedAssetsInterval = null;
        private SerializedProperty m_UseSystemUnloadUnusedAssets = null;
        private SerializedProperty m_AssetAutoReleaseInterval = null;
        private SerializedProperty m_AssetCapacity = null;
        private SerializedProperty m_AssetExpireTime = null;
        private SerializedProperty m_AssetPriority = null;
        private SerializedProperty m_DownloadingMaxNum = null;
        private SerializedProperty m_FailedTryAgain = null;
        private SerializedProperty m_LoadResourceAgentHelper = null;
        private SerializedProperty m_ResourceHelper = null;

        private int m_ResourceModeIndex = 0;
        private int m_VerifyIndex = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            ResourceComponent t = (ResourceComponent)target;

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
            {
                if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject))
                {
                    EditorGUILayout.EnumPopup("Resource Mode", t.PlayMode);
                    EditorGUILayout.EnumPopup("VerifyLevel", t.FileVerifyLevel);
                }
                else
                {
                    int selectedIndex = EditorGUILayout.Popup("Resource Mode", m_ResourceModeIndex, _resourceModeNames);
                    if (selectedIndex != m_ResourceModeIndex)
                    {
                        m_ResourceModeIndex = selectedIndex;
                        m_PlayMode.enumValueIndex = selectedIndex;
                    }

                    int selectedVerifyIndex = EditorGUILayout.Popup("VerifyLevel", m_VerifyIndex, _verifyLevelNames);
                    if (selectedVerifyIndex != m_VerifyIndex)
                    {
                        m_VerifyIndex = selectedVerifyIndex;
                        m_FileVerifyLevel.enumValueIndex = selectedVerifyIndex;
                    }
                }

                EditorGUILayout.PropertyField(m_DefaultPackageName);

                // m_ReadWritePathType.enumValueIndex = (int)(ReadWritePathType)EditorGUILayout.EnumPopup("Read-Write Path Type", t.ReadWritePathType);
            }
            // EditorGUILayout.PropertyField(m_UpdatableWhilePlaying);

            EditorGUI.EndDisabledGroup();

            int milliseconds = EditorGUILayout.DelayedIntField("Milliseconds", m_Milliseconds.intValue);
            if (milliseconds != m_Milliseconds.intValue)
            {
                m_Milliseconds.longValue = milliseconds;
            }

            EditorGUILayout.PropertyField(m_UseSystemUnloadUnusedAssets);

            float minUnloadUnusedAssetsInterval =
                EditorGUILayout.Slider("Min Unload Unused Assets Interval", m_MinUnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (Math.Abs(minUnloadUnusedAssetsInterval - m_MinUnloadUnusedAssetsInterval.floatValue) > 0.01f)
            {
                m_MinUnloadUnusedAssetsInterval.floatValue = minUnloadUnusedAssetsInterval;
            }

            float maxUnloadUnusedAssetsInterval =
                EditorGUILayout.Slider("Max Unload Unused Assets Interval", m_MaxUnloadUnusedAssetsInterval.floatValue, 0f, 3600f);
            if (Math.Abs(maxUnloadUnusedAssetsInterval - m_MaxUnloadUnusedAssetsInterval.floatValue) > 0.01f)
            {
                m_MaxUnloadUnusedAssetsInterval.floatValue = maxUnloadUnusedAssetsInterval;
            }

            float downloadingMaxNum = EditorGUILayout.Slider("Max Downloading Num", m_DownloadingMaxNum.intValue, 1f, 48f);
            if (Math.Abs(downloadingMaxNum - m_DownloadingMaxNum.intValue) > 0.001f)
            {
                m_DownloadingMaxNum.intValue = (int)downloadingMaxNum;
            }

            float failedTryAgain = EditorGUILayout.Slider("Max FailedTryAgain Count", m_FailedTryAgain.intValue, 1f, 48f);
            if (Math.Abs(failedTryAgain - m_FailedTryAgain.intValue) > 0.001f)
            {
                m_FailedTryAgain.intValue = (int)failedTryAgain;
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            {
                float assetAutoReleaseInterval = EditorGUILayout.DelayedFloatField("Asset Auto Release Interval", m_AssetAutoReleaseInterval.floatValue);
                if (Math.Abs(assetAutoReleaseInterval - m_AssetAutoReleaseInterval.floatValue) > 0.01f)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetAutoReleaseInterval = assetAutoReleaseInterval;
                    }
                    else
                    {
                        m_AssetAutoReleaseInterval.floatValue = assetAutoReleaseInterval;
                    }
                }

                int assetCapacity = EditorGUILayout.DelayedIntField("Asset Capacity", m_AssetCapacity.intValue);
                if (assetCapacity != m_AssetCapacity.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetCapacity = assetCapacity;
                    }
                    else
                    {
                        m_AssetCapacity.intValue = assetCapacity;
                    }
                }

                float assetExpireTime = EditorGUILayout.DelayedFloatField("Asset Expire Time", m_AssetExpireTime.floatValue);
                if (Math.Abs(assetExpireTime - m_AssetExpireTime.floatValue) > 0.01f)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetExpireTime = assetExpireTime;
                    }
                    else
                    {
                        m_AssetExpireTime.floatValue = assetExpireTime;
                    }
                }

                int assetPriority = EditorGUILayout.DelayedIntField("Asset Priority", m_AssetPriority.intValue);
                if (assetPriority != m_AssetPriority.intValue)
                {
                    if (EditorApplication.isPlaying)
                    {
                        t.AssetPriority = assetPriority;
                    }
                    else
                    {
                        m_AssetPriority.intValue = assetPriority;
                    }
                }
            }

            EditorGUILayout.PropertyField(m_ResourceHelper);
            EditorGUILayout.PropertyField(m_LoadResourceAgentHelper);

            EditorGUI.EndDisabledGroup();

            if (EditorApplication.isPlaying && IsPrefabInHierarchy(t.gameObject) && t.IsInitialized)
            {
                EditorGUILayout.LabelField("Unload Unused Assets",
                    Utility.Text.Format("{0:F2} / {1:F2}", t.LastUnloadUnusedAssetsOperationElapseSeconds, t.MaxUnloadUnusedAssetsInterval));
                EditorGUILayout.LabelField("Read-Only Path", t?.ReadOnlyPath?.ToString());
                EditorGUILayout.LabelField("Read-Write Path", t?.ReadWritePath?.ToString());
                // EditorGUILayout.LabelField("Applicable Game Version", t.ApplicableGameVersion ?? "<Unknwon>");
            }

            serializedObject.ApplyModifiedProperties();

            Repaint();
        }

        protected override void OnCompileComplete()
        {
            base.OnCompileComplete();

            RefreshTypeNames();
        }

        private void OnEnable()
        {
            m_PlayMode = serializedObject.FindProperty("m_PlayMode");
            m_DefaultPackageName = serializedObject.FindProperty("m_DefaultPackageName");
            // m_UpdatableWhilePlaying = serializedObject.FindProperty("m_UpdatableWhilePlaying");
            m_FileVerifyLevel = serializedObject.FindProperty("m_FileVerifyLevel");
            m_Milliseconds = serializedObject.FindProperty("m_Milliseconds");
            // m_ReadWritePathType = serializedObject.FindProperty("m_ReadWritePathType");
            m_MinUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MinUnloadUnusedAssetsInterval");
            m_MaxUnloadUnusedAssetsInterval = serializedObject.FindProperty("m_MaxUnloadUnusedAssetsInterval");
            m_UseSystemUnloadUnusedAssets = serializedObject.FindProperty("m_UseSystemUnloadUnusedAssets");
            m_AssetAutoReleaseInterval = serializedObject.FindProperty("m_AssetAutoReleaseInterval");
            m_AssetCapacity = serializedObject.FindProperty("m_AssetCapacity");
            m_AssetExpireTime = serializedObject.FindProperty("m_AssetExpireTime");
            m_AssetPriority = serializedObject.FindProperty("m_AssetPriority");
            m_DownloadingMaxNum = serializedObject.FindProperty("m_DownloadingMaxNum");
            m_FailedTryAgain = serializedObject.FindProperty("m_FailedTryAgain");
            m_LoadResourceAgentHelper = serializedObject.FindProperty("m_LoadResourceAgentHelper");
            m_ResourceHelper = serializedObject.FindProperty("m_ResourceHelper");

            RefreshModes();
            RefreshTypeNames();
        }

        private void RefreshModes()
        {
            m_ResourceModeIndex = m_PlayMode.enumValueIndex > 0 ? m_PlayMode.enumValueIndex : 0;
            m_VerifyIndex = m_FileVerifyLevel.enumValueIndex > 0 ? m_FileVerifyLevel.enumValueIndex : 0;
        }

        private void RefreshTypeNames()
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
