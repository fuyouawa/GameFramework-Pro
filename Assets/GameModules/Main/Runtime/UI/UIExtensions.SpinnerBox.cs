using System;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public static partial class UIExtensions
    {
        private static UIForm s_lastSpinnerBox;

        public static async UniTask BeginSpinnerBoxAsync(this UIComponent uiComponent,
            Func<string> descriptionGetter,
            Func<int> percentageGetter,
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
            spinnerBox.PercentageGetter = percentageGetter;
        }

        public static void UpdateSpinnerBox(this UIComponent uiComponent,
            Func<string> descriptionGetter,
            Func<int> percentageGetter)
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

            spinnerBox.DescriptionGetter = descriptionGetter;
            spinnerBox.PercentageGetter = percentageGetter;
        }

        public static async UniTask EndSpinnerBoxAsync(this UIComponent uiComponent)
        {
            await uiComponent.CloseUIPanelAsync(s_lastSpinnerBox);
            s_lastSpinnerBox = null;
        }
    }
}
