using EasyToolKit.Core;
using EasyToolKit.Inspector;
using UnityEngine;

namespace GameMain.Runtime
{
    [EasyInspector]
    [ScriptableObjectSingletonAssetPath("Assets/Resources/Configs/UI")]
    public class UISpinnerBoxConfigAsset : ScriptableObjectSingleton<UISpinnerBoxConfigAsset>
    {
        [Title("资源")]
        [LabelText("资源引用")]
        [SerializeField] private AssetReference _assetReference = new AssetReference(Constant.Package.Builtin, "UI_SpinnerBox");

        [Title("默认值")]
        [LabelText("分组名")]
        [SerializeField] private string _defaultGroupName = "Popup";

        public AssetReference AssetReference => _assetReference;
        public string DefaultGroupName => _defaultGroupName;
    }
}
