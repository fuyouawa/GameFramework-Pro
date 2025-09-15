using System;
using TMPro;
using UnityEngine;

namespace GameMain.Runtime
{
    public class UISpinnerBox : UIPanel
    {
        [SerializeField] private RectTransform _circlesGroup;
        [SerializeField] private TextMeshProUGUI _percentageText;
        [SerializeField] private TextMeshProUGUI _descriptionText;

        private int _percentage;

        public int Percentage
        {
            get => _percentage;
            set
            {
                _percentage = value;
                _percentageText.text = $"{_percentage}%";
            }
        }

        public string Description
        {
            get => _descriptionText.text;
            set => _descriptionText.text = value;
        }

        public Func<string> DescriptionGetter;
        public Func<int> PercentageGetter;

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

            if (PercentageGetter != null)
            {
                Percentage = PercentageGetter();
            }
        }
    }
}
