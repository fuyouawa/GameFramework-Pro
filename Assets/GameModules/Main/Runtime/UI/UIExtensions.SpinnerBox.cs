using System;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static partial class UIExtensions
    {
        private static UIForm s_lastSpinnerBox;

        public static UniTask BeginSpinnerBoxAsync(this UIComponent uiComponent,
            string descriptionGetter,
            int initialPercentage)
        {
            return BeginSpinnerBoxAsync(uiComponent, () => descriptionGetter, initialPercentage);
        }

        public static UniTask BeginSpinnerBoxAsync(this UIComponent uiComponent,
            Func<string> descriptionGetter,
            int initialPercentage)
        {
            return BeginSpinnerBoxAsync(uiComponent, descriptionGetter, initialPercentage,
                UISpinnerBoxConfigAsset.Instance.DefaultGroupName);
        }

        public static async UniTask BeginSpinnerBoxAsync(this UIComponent uiComponent,
            Func<string> descriptionGetter,
            int initialPercentage,
            string groupName)
        {
            if (s_lastSpinnerBox != null)
            {
                throw new InvalidOperationException("Last spinner box is showing.");
            }

            var assetReference = UISpinnerBoxConfigAsset.Instance.AssetReference;
            var form = await uiComponent.OpenUIFormAsync(assetReference.AssetName, groupName,
                assetReference.PackageName);
            if (form.Logic is not UISpinnerBox spinnerBox)
            {
                throw new InvalidOperationException(
                    $"UI form logic type '{form.Logic.GetType()}' is not '{typeof(UISpinnerBox)}'.");
            }

            s_lastSpinnerBox = form;
            spinnerBox.DescriptionGetter = descriptionGetter;
            spinnerBox.Percentage = initialPercentage;
        }

        public static bool IsSpinnerBoxShowing(this UIComponent uiComponent)
        {
            return s_lastSpinnerBox != null;
        }

        public static UniTask UpdateSpinnerBoxAsync(this UIComponent uiComponent,
            string descriptionGetter,
            float destinationPercentage,
            float duration = 0.2f)
        {
            return UpdateSpinnerBoxAsync(uiComponent, () => descriptionGetter, destinationPercentage, duration);
        }

        public static UniTask UpdateSpinnerBoxAsync(this UIComponent uiComponent,
            float destinationPercentage,
            float duration = 0.2f)
        {
            return UpdateSpinnerBoxAsync(uiComponent, (Func<string>)null, destinationPercentage, duration);
        }

        public static UniTask UpdateSpinnerBoxAsync(this UIComponent uiComponent,
            [CanBeNull] Func<string> descriptionGetter,
            float destinationPercentage,
            float duration = 0.2f)
        {
            if (s_lastSpinnerBox == null)
            {
                throw new InvalidOperationException("Spinner box is not showing, must use BeginSpinnerBoxAsync first.");
            }

            if (s_lastSpinnerBox.Logic is not UISpinnerBox spinnerBox)
            {
                throw new InvalidOperationException(
                    $"UI form logic type '{s_lastSpinnerBox.Logic.GetType()}' is not '{typeof(UISpinnerBox)}'.");
            }

            if (descriptionGetter != null)
            {
                spinnerBox.DescriptionGetter = descriptionGetter;
            }

            var arrivedTcs = new UniTaskCompletionSource();
            spinnerBox.SetDestinationPercentage(destinationPercentage, duration, () => { arrivedTcs.TrySetResult(); });

            return arrivedTcs.Task;
        }

        public static async UniTask EndSpinnerBoxAsync(this UIComponent uiComponent)
        {
            await uiComponent.CloseUIPanelAsync(s_lastSpinnerBox);
            s_lastSpinnerBox = null;
        }
    }
}
