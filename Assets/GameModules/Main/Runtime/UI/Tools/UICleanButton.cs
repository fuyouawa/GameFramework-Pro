using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    [AddComponentMenu("UI/Clean Button")]
    public class UICleanButton : Button
    {
        [SerializeField] private float _fadeTime = 0.2f;
        [SerializeField] private float _onHoverAlpha;
        [SerializeField] private float _onClickAlpha;

        private CanvasGroup _canvasGroup;
        private Tweener _previousTweener;

        protected override void Awake()
        {
            _canvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            _previousTweener?.Kill();
            _previousTweener = _canvasGroup.DOFade(_onHoverAlpha, _fadeTime);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            _previousTweener?.Kill();
            _previousTweener = _canvasGroup.DOFade(1.0f, _fadeTime);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            _canvasGroup.alpha = _onClickAlpha;
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            _canvasGroup.alpha = 1.0f;
        }
    }
}
