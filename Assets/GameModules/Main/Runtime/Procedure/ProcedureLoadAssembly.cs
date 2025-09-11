using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if ENABLE_HYBRIDCLR
using HybridCLR;
#endif
using UnityEngine;
using System.Reflection;
using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using GameFramework.Resource;
using HybridCLR;
using UnityGameFramework.Runtime;
using YooAsset;
using PlayMode = GameFramework.Resource.PlayMode;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程加载器 - 代码初始化
    /// </summary>
    public class ProcedureLoadAssembly : ProcedureBase
    {
        private int m_LoadAssetCount;
        private int m_LoadMetadataAssetCount;
        private int m_FailureAssetCount;
        private int m_FailureMetadataAssetCount;
        private bool m_LoadAssemblyComplete;
        private bool m_LoadMetadataAssemblyComplete;
        private bool m_LoadAssemblyWait;
        private bool m_LoadMetadataAssemblyWait;
        private List<Assembly> _hotUpdateAssemblys;
        private IFsm<IProcedureManager> m_ProcedureOwner;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);
            m_ProcedureOwner = procedureOwner;
            m_LoadAssemblyComplete = false;
            _hotUpdateAssemblys = new List<Assembly>();

#if !UNITY_EDITOR
            m_LoadMetadataAssemblyComplete = false;
            LoadMetadataForAOTAssembly();
#else
            m_LoadMetadataAssemblyComplete = true;
#endif

            if (GameEntry.Resource.PlayMode == PlayMode.EditorSimulateMode)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var hotUpdateAssemblyName in GameSettings.Instance.HotUpdateAssemblyNames)
                    {
                        if (hotUpdateAssemblyName == $"{assembly.GetName().Name}")
                        {
                            _hotUpdateAssemblys.Add(assembly);
                        }
                    }

                    if (_hotUpdateAssemblys.Count == GameSettings.Instance.HotUpdateAssemblyNames.Count)
                    {
                        break;
                    }
                }
            }
            else
            {
                var callbacks = new LoadAssetCallbacks(OnLoadAssemblyAssetSuccess, OnLoadAssemblyAssetFailure);
                foreach (string hotUpdateAssemblyName in GameSettings.Instance.HotUpdateAssemblyNames)
                {
                    m_LoadAssetCount++;
                    GameEntry.Resource.LoadAsset(hotUpdateAssemblyName, callbacks, assetType:typeof(TextAsset));
                }

                m_LoadAssemblyWait = true;
            }

            if (m_LoadAssetCount == 0)
            {
                m_LoadAssemblyComplete = true;
            }
        }

        private void OnLoadAssemblyAssetSuccess(string assetName, object asset, float duration, object userdata)
        {
            var textAsset = asset as TextAsset;

            m_LoadAssetCount--;

            if (textAsset == null)
            {
                Log.Warning($"Load Assembly failed.");
                return;
            }

            Log.Debug($"LoadAssetSuccess, assetName: [ {assetName} ]");

            try
            {
                var assembly = Assembly.Load(textAsset.bytes);
                _hotUpdateAssemblys.Add(assembly);
                Log.Debug($"Assembly [ {assembly.GetName().Name} ] loaded");
            }
            catch (Exception e)
            {
                m_FailureAssetCount++;
                Log.Fatal(e);
                throw;
            }
            finally
            {
                m_LoadAssemblyComplete = m_LoadAssemblyWait && 0 == m_LoadAssetCount;
            }
        }

        private void OnLoadAssemblyAssetFailure(string assetName, LoadResourceStatus status, string error, object userdata)
        {
            throw new NotImplementedException();
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!m_LoadAssemblyComplete)
            {
                return;
            }

            if (!m_LoadMetadataAssemblyComplete)
            {
                return;
            }

            AllAssemblyLoadComplete();
        }

        private void AllAssemblyLoadComplete()
        {
            _hotUpdateAssemblys.Sort(GameSettings.Instance.HotUpdateAssemblyComparison);
            foreach (var assembly in _hotUpdateAssemblys)
            {
                EntryAssembly(assembly);
            }
            Log.Info("Load assemblies complete.");
            ChangeState<ProcedureStartGame>(m_ProcedureOwner);
        }

        private void EntryAssembly(Assembly assembly)
        {
            var entries = assembly.GetTypes()
                .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                .Where(method => method.IsDefined(typeof(HotUpdateEntryAttribute)));
            foreach (var entry in entries)
            {
                entry.Invoke(null, null);
            }
        }

        /// <summary>
        /// 加载代码资源成功回调。
        /// </summary>
        /// <param name="textAsset">资源操作句柄。</param>
        private void LoadAssetSuccess(TextAsset textAsset)
        {
        }

        /// <summary>
        /// 为Aot Assembly加载原始metadata， 这个代码放Aot或者热更新都行。
        /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行。
        /// </summary>
        public void LoadMetadataForAOTAssembly()
        {
            // 可以加载任意aot assembly的对应的dll。但要求dll必须与unity build过程中生成的裁剪后的dll一致，而不能直接使用原始dll。
            // 我们在BuildProcessor_xxx里添加了处理代码，这些裁剪后的dll在打包时自动被复制到 {项目目录}/HybridCLRData/AssembliesPostIl2CppStrip/{Target} 目录。

            // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            if (GameSettings.Instance.AOTMetaAssemblyNames.Count == 0)
            {
                m_LoadMetadataAssemblyComplete = true;
                return;
            }

            foreach (string aotDllName in GameSettings.Instance.AOTMetaAssemblyNames)
            {
                var assetLocation = aotDllName;
                Log.Debug($"LoadMetadataAsset: [ {assetLocation} ]");
                m_LoadMetadataAssetCount++;
                // GameModule.Resource.LoadAsset<TextAsset>(assetLocation, LoadMetadataAssetSuccess);
            }

            m_LoadMetadataAssemblyWait = true;
        }

        /// <summary>
        /// 加载元数据资源成功回调。
        /// </summary>
        /// <param name="textAsset">资源操作句柄。</param>
        private void LoadMetadataAssetSuccess(TextAsset textAsset)
        {
            m_LoadMetadataAssetCount--;

            if (null == textAsset)
            {
                Log.Debug($"LoadMetadataAssetSuccess:Load Metadata failed.");
                return;
            }

            string assetName = textAsset.name;
            Log.Debug($"LoadMetadataAssetSuccess, assetName: [ {assetName} ]");

            try
            {
                byte[] dllBytes = textAsset.bytes;
                // 加载assembly对应的dll，会自动为它hook。一旦Aot泛型函数的native函数不存在，用解释器版本代码
                HomologousImageMode mode = HomologousImageMode.SuperSet;
                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes,mode);
                Log.Warning($"LoadMetadataForAOTAssembly:{assetName}. mode:{mode} ret:{err}");
            }
            catch (Exception e)
            {
                m_FailureMetadataAssetCount++;
                Log.Fatal(e.Message);
                throw;
            }
            finally
            {
                m_LoadMetadataAssemblyComplete = m_LoadMetadataAssemblyWait && 0 == m_LoadMetadataAssetCount;
            }
        }
    }
}
