using System;
using DG.Tweening;
using EasyToolKit.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Runtime
{
    public class UISpinnerBox : UIPanel
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TextMeshProUGUI _percentageText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private float _percentage;
        private float _destinationPercentage;
        private Tweener _destinationPercentageTweener;

        public float Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                _percentageText.text = $"{(int)(_percentage * 100f)}%";
            }
        }

        public string Description
        {
            get => _descriptionText.text;
            set => _descriptionText.text = value;
        }

        public float BackgroundAlpha
        {
            get => _backgroundImage.color.a;
            set => _backgroundImage.color = _backgroundImage.color.SetA(value);
        }

        public Func<string> DescriptionGetter;

        public void SetDestinationPercentage(float percentage, float duration, Action arrived)
        {
            if (_destinationPercentageTweener != null)
            {
                _destinationPercentageTweener.Kill();
                _destinationPercentageTweener = null;
            }

            _destinationPercentageTweener = DOTween.To(
                () => Percentage,
                value => Percentage = value,
                percentage, duration)
                .OnKill(() => arrived?.Invoke());
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            Percentage = 0;
            Description = "";
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);

            if (DescriptionGetter != null)
            {
                Description = DescriptionGetter();
            }
        }
    }
}
