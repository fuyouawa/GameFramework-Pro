using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public abstract class UIPanel : UIFormLogic
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        public CanvasGroup CanvasGroup => _canvasGroup;
        public RectTransform RectTransform => _rectTransform;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = Vector2.one;
            _rectTransform.anchoredPosition = Vector2.zero;
            _rectTransform.sizeDelta = Vector2.zero;
        }
    }
}
