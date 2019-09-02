#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Custom.Tool.AutoBuild;

namespace Custom.Tool
{
    public class UpgradeVersionPopWindow : APopUpWindow
    {
        private static UpgradeVersionPopWindow _window;
        private string _version;
        private string _suggestionVersion;
        private string _projectSettingsVersion;
        private string _text;
        public static void OpenWindow()
        {
            _window = GetWindow<UpgradeVersionPopWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(270, 200);
            _window.titleContent = new GUIContent("Pop Up window", EditorIcons.RulerRect.Active);
            _window.Show();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _version = VersionManager.Instance.GetVersion();
            _suggestionVersion = VersionManager.Instance.GetSuggestionVersion();
            _projectSettingsVersion = PlayerSettings.bundleVersion;
            _text = _suggestionVersion;

        }

        [OnInspectorGUI]
        private void CreateLabel()
        {
            EditorGUILayout.LabelField("What is the next version?", SirenixGUIStyles.BoldLabelCentered);
            EditorGUILayout.LabelField("File version : " + _version, SirenixGUIStyles.CenteredBlackMiniLabel);

            GUIStyle s = new GUIStyle(SirenixGUIStyles.CenteredBlackMiniLabel);
            s.normal.textColor = Color.magenta;
            EditorGUILayout.LabelField("Project settings version : " + _projectSettingsVersion, s);

            EditorGUILayout.LabelField("Suggested version : " + _suggestionVersion, SirenixGUIStyles.CenteredGreyMiniLabel);
           
            GUILayout.Space(30);
            _text = EditorGUILayout.TextField(_text, SirenixGUIStyles.CenteredTextField);           
            GUILayout.Space(10);
        }

        [ButtonGroup("Group"), LabelText("Save"), GUIColor(0, 1, 0)]
        private void Save()
        {
            VersionManager.Instance.SetVersion(_text);
            GetWindow<UpgradeVersionPopWindow>().Close();
        }
        [ButtonGroup("Group"), LabelText("Cancel"), GUIColor(1, 0, 0)]
        private void Cancel()
        {
           // UnityEngine.Debug.LogError("<b><color=red> File not created because it was canceled. </color></b>");
            GetWindow<UpgradeVersionPopWindow>().Close();
        }
    }
}
#endif
