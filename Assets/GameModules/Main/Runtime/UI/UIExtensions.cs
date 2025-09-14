using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameFramework;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public enum UIMessageBoxButtons
    {
        Ok,
        OkCancel,
        Yes,
        YesNo,
        YesNoCancel,
    }

    public enum UIMessageBoxType
    {
        Tip,
        Warn,
        Error
    }

    public enum UIMessageBoxStyle
    {
        Builtin
    }

    public static class UIExtensions
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
            int? customPriority = null,
            bool pauseCoveredUIForm = false,
            object userData = null)
        {
            var serialId = uiComponent.OpenUIForm(uiFormAssetName, uiGroupName, customPackageName, customPriority,
                pauseCoveredUIForm, userData);
            if (UIFormOpenCompletedTcsBySerialId.TryGetValue(serialId, out var tcs))
            {
                return tcs.Task;
            }

            tcs = new UniTaskCompletionSource<UIForm>();
            UIFormOpenCompletedTcsBySerialId.Add(serialId, tcs);
            return tcs.Task;
        }

        public static UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            UIMessageBoxButtons buttons = UIMessageBoxButtons.Ok)
        {
            return uiComponent.ShowMessageBoxAsync(message, UIMessageBoxType.Tip, buttons);
        }

        public static UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            UIMessageBoxType type,
            UIMessageBoxButtons buttons = UIMessageBoxButtons.Ok)
        {
            return uiComponent.ShowMessageBoxAsync(message, UIMessageBoxConfigAsset.Instance.DefaultGroupName, type,
                buttons);
        }

        public static UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            string groupName,
            UIMessageBoxType type,
            UIMessageBoxButtons buttons = UIMessageBoxButtons.Ok)
        {
            return uiComponent.ShowMessageBoxAsync(message, GetMessageBoxTitle(type), groupName, type,
                GetMessageBoxButtonTexts(buttons));
        }

        public static UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            string title,
            params string[] buttonTexts)
        {
            return uiComponent.ShowMessageBoxAsync(message, title, UIMessageBoxType.Tip, buttonTexts);
        }

        public static UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            string title,
            UIMessageBoxType type,
            params string[] buttonTexts)
        {
            return uiComponent.ShowMessageBoxAsync(message, title, UIMessageBoxConfigAsset.Instance.DefaultGroupName,
                type, buttonTexts);
        }

        public static async UniTask<int> ShowMessageBoxAsync(this UIComponent uiComponent,
            string message,
            string title,
            string groupName,
            UIMessageBoxType type,
            params string[] buttonTexts)
        {
            var assetReference = UIMessageBoxConfigAsset.Instance.AssetReference;
            var form = await uiComponent.OpenUIFormAsync(assetReference.AssetName, groupName,
                assetReference.PackageName);
            var messageBox = form.Logic as UIMessageBox;
            if (messageBox == null)
            {
                throw new InvalidOperationException(
                    $"UI form logic type '{form.Logic.GetType()}' is not '{typeof(UIMessageBox)}'.");
            }

            messageBox.Message = message;
            messageBox.Title = title;
            messageBox.MessageBoxType = type;

            var clickCompletedTcs = new UniTaskCompletionSource<int>();
            for (int i = 0; i < buttonTexts.Length; i++)
            {
                int localI = i;
                messageBox.AddButton(buttonTexts[i], _ => { clickCompletedTcs.TrySetResult(localI); });
            }

            var index = await clickCompletedTcs.Task;

            var closeCompletedTcs = new UniTaskCompletionSource();
            messageBox.CloseCompleted += () => { closeCompletedTcs.TrySetResult(); };

            uiComponent.CloseUIForm(form);

            await closeCompletedTcs.Task;

            return index;
        }

        private static string[] GetMessageBoxButtonTexts(UIMessageBoxButtons buttons)
        {
            var config = UIMessageBoxConfigAsset.Instance;
            switch (buttons)
            {
                case UIMessageBoxButtons.Ok:
                    return config.OkTexts.ToArray();
                case UIMessageBoxButtons.OkCancel:
                    return config.OkCancelTexts.ToArray();
                case UIMessageBoxButtons.Yes:
                    return config.YesTexts.ToArray();
                case UIMessageBoxButtons.YesNo:
                    return config.YesNoTexts.ToArray();
                case UIMessageBoxButtons.YesNoCancel:
                    return config.YesNoCancelTexts.ToArray();
                default:
                    throw new ArgumentOutOfRangeException(nameof(buttons), buttons, null);
            }
        }

        private static string GetMessageBoxTitle(UIMessageBoxType type)
        {
            var config = UIMessageBoxConfigAsset.Instance;
            switch (type)
            {
                case UIMessageBoxType.Tip:
                    return config.TipTitle;
                case UIMessageBoxType.Warn:
                    return config.WarnTitle;
                case UIMessageBoxType.Error:
                    return config.ErrorTitle;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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
