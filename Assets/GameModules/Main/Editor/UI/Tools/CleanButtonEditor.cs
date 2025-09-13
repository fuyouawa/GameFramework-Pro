using GameMain.Runtime;
using UnityEditor;
using UnityEditor.UI;

namespace GameMain.Editor
{
    [CustomEditor(typeof(UICleanButton), true)]
    [CanEditMultipleObjects]
    public class CleanButtonEditor : ButtonEditor
    {
        private SerializedProperty _fadeTimeProperty;
        private SerializedProperty _onHoverAlphaProperty;
        private SerializedProperty _onClickAlphaProperty;

        protected override void OnEnable()
        {
            base.OnEnable();
            _fadeTimeProperty = serializedObject.FindProperty("_fadeTime");
            _onHoverAlphaProperty = serializedObject.FindProperty("_onHoverAlpha");
            _onClickAlphaProperty = serializedObject.FindProperty("_onClickAlpha");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField("Clean Settings", EditorStyles.boldLabel);
            serializedObject.Update();

            EditorGUILayout.PropertyField(_fadeTimeProperty);
            EditorGUILayout.PropertyField(_onHoverAlphaProperty);
            EditorGUILayout.PropertyField(_onClickAlphaProperty);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
