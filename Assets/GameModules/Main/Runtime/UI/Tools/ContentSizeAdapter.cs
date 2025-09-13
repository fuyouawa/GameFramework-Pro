using System;
using System.Collections.Generic;
using EasyToolKit.Inspector;
using UnityEngine;

namespace GameMain.Runtime
{
    [EasyInspector]
    [AddComponentMenu("Layout/Content Size Adapter")]
    public class ContentSizeAdapter : MonoBehaviour
    {
        [Serializable]
        [HideLabel]
        class BindingItem
        {
            [LabelText("绑定对象")]
            public RectTransform Target;
            [Range(0f, 2f)]
            [LabelText("系数")]
            public float Coefficient = 1f;
            [LabelText("匹配宽度or高度")]
            public bool MatchWidthOrHeight;

            [NonSerialized] public Vector2 OriginSize;
        }

        [LabelText("目标对象")]
        [SerializeField] private RectTransform _target;

        [LabelText("绑定列表")]
        [ListDrawerSettings(ShowIndexLabel = false)]
        [SerializeField] private List<BindingItem> _bindingItems = new List<BindingItem>();

        private Vector2 _originSize;

        private void Awake()
        {
            foreach (var item in _bindingItems)
            {
                item.OriginSize = item.Target.sizeDelta;
            }

            _originSize = _target.sizeDelta;
        }

        private void Update()
        {
            Vector2 totalDifferentSize = Vector2.zero;
            foreach (var item in _bindingItems)
            {
                var differentSize = item.Target.sizeDelta - item.OriginSize;
                differentSize *= item.Coefficient;
                totalDifferentSize += item.MatchWidthOrHeight ? new Vector2(differentSize.x, 0) : new Vector2(0, differentSize.y);
            }

            _target.sizeDelta = _originSize + totalDifferentSize;
        }
    }
}
