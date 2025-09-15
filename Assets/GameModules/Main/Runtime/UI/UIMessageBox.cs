using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameMain.Runtime
{
    public class UIMessageBox : UIPanel
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _messageText;
        [SerializeField] private RectTransform _buttonGroup;

        [SerializeField] private AssetReference _buttonAsset;

        private GameObject _buttonPrefab;
        private readonly Queue<(string text, Action<Button> onClick)> _addButtonsQueue = new Queue<(string text, Action<Button> onClick)>();

        public string Title
        {
            get => _titleText.text;
            set => _titleText.text = value;
        }

        public string Message
        {
            get => _messageText.text;
            set => _messageText.text = value;
        }

        public UIMessageBoxType MessageBoxType
        {
            set
            {
                //TODO MessageBoxType
            }
        }

        public void AddButton(string text, Action<Button> onClick)
        {
            _addButtonsQueue.Enqueue((text, onClick));
        }

        public void ClearButtons()
        {
            for (int i = _buttonGroup.childCount - 1; i >= 0; i--)
            {
                Destroy(_buttonGroup.GetChild(i).gameObject);
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            ClearButtons();
            _buttonAsset.LoadAssetAsync<GameObject>()
                .ContinueWith(o => _buttonPrefab = o)
                .Forget();
        }

        private void Update()
        {
            if (_buttonPrefab != null)
            {
                while (_addButtonsQueue.Count > 0)
                {
                    var (text, onClick) = _addButtonsQueue.Dequeue();
                    var instantiate = Instantiate(_buttonPrefab, _buttonGroup);
                    instantiate.GetComponentInChildren<TextMeshProUGUI>().text = text;

                    var button = instantiate.GetComponent<Button>();
                    button.onClick.AddListener(() => onClick(button));
                }
            }
        }
    }
}
