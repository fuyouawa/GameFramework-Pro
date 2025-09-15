using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static partial class UIExtensions
    {
        private static readonly Dictionary<int, UniTaskCompletionSource<UIForm>> UIFormOpenCompletedTcsBySerialId =
            new Dictionary<int, UniTaskCompletionSource<UIForm>>();

        static UIExtensions()
        {
            GameEntry.Event.Subscribe<OpenUIFormSuccessEventArgs>(OnEvent);
            GameEntry.Event.Subscribe<OpenUIFormFailureEventArgs>(OnEvent);
        }

        /// <summary>
        /// 异步打开界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="customPriority">加载界面资源的优先级。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面。</returns>
        public static UniTask<UIForm> OpenUIFormAsync(this UIComponent uiComponent,
            string uiFormAssetName,
            string uiGroupName,
            string customPackageName = "",
            bool pauseCoveredUIForm = false,
            object userData = null)
        {
            var serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, customPackageName, Constant.AssetPriority.UIFormAsset,
                pauseCoveredUIForm, userData);
            if (UIFormOpenCompletedTcsBySerialId.TryGetValue(serialId, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<UIForm>();
            UIFormOpenCompletedTcsBySerialId.Add(serialId, tcs);
            return tcs.Task;
        }

        public static async UniTask CloseUIPanelAsync(this UIComponent uiComponent, UIForm form)
        {
            var closeCompletedTcs = new UniTaskCompletionSource();
            if (form.Logic is not UIPanel panel)
            {
                throw new ArgumentException($"UI form logic type '{form.Logic.GetType()}' is not '{typeof(UIPanel)}'.");
            }
            panel.CloseCompleted += () => { closeCompletedTcs.TrySetResult(); };

            uiComponent.CloseUIForm(form);

            await closeCompletedTcs.Task;
        }

        private static void OnEvent(object sender, OpenUIFormSuccessEventArgs e)
        {
            var tcs = UIFormOpenCompletedTcsBySerialId[e.UIForm.SerialId];
            UIFormOpenCompletedTcsBySerialId.Remove(e.UIForm.SerialId);
            tcs.TrySetResult(e.UIForm);
        }

        private static void OnEvent(object sender, OpenUIFormFailureEventArgs e)
        {
            var tcs = UIFormOpenCompletedTcsBySerialId[e.SerialId];
            UIFormOpenCompletedTcsBySerialId.Remove(e.SerialId);
            tcs.TrySetException(new GameFrameworkException(e.ErrorMessage));
        }
    }
}
