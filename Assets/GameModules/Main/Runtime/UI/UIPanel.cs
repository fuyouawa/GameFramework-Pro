using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    public enum UIAnimation
    {
        FadeIn,
        FadeOut
    }

    public abstract class UIPanel : UIFormLogic
    {
        private CanvasGroup _canvasGroup;
        private RectTransform _rectTransform;

        public event Action OpenCompleted;
        public event Action CloseCompleted;

        public CanvasGroup CanvasGroup => _canvasGroup;
        public RectTransform RectTransform => _rectTransform;

        protected virtual UIAnimation OpenAnimation => UIAnimation.FadeIn;
        protected virtual UIAnimation CloseAnimation => UIAnimation.FadeOut;

        protected virtual float OpenDuration => 0.3f;
        protected virtual float CloseDuration => 0.3f;

        private Tweener _previousTweener;

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

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            DoAnimation(OpenAnimation, OpenDuration, () =>
            {
                OnOpenCompleted();
                OpenCompleted?.Invoke();
            });
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            DoAnimation(CloseAnimation, CloseDuration, () =>
            {
                OnCloseCompleted();
                CloseCompleted?.Invoke();
            });
        }

        protected virtual void OnOpenCompleted() {}
        protected virtual void OnCloseCompleted() {}

        private void DoAnimation(UIAnimation uiAnimation, float duration, Action completed)
        {
            if (_previousTweener != null && _previousTweener.IsPlaying())
            {
                _previousTweener.Kill();
                _previousTweener = null;
            }

            switch (uiAnimation)
            {
                case UIAnimation.FadeIn:
                    _canvasGroup.alpha = 0f;
                    _canvasGroup.blocksRaycasts = false;
                    _previousTweener = _canvasGroup.DOFade(1f, duration).OnKill(() =>
                    {
                        _canvasGroup.blocksRaycasts = true;
                        completed?.Invoke();
                    });
                    break;
                case UIAnimation.FadeOut:
                    _canvasGroup.alpha = 1f;
                    _canvasGroup.blocksRaycasts = false;
                    _previousTweener = _canvasGroup.DOFade(0f, duration).OnKill(() => completed?.Invoke());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(uiAnimation), uiAnimation, null);
            }
        }
    }
}
