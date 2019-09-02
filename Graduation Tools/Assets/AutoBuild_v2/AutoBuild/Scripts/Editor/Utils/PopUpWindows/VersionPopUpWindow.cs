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
    public class VersionPopUpWindow : APopUpWindow
    {
        private static VersionPopUpWindow _window;

        public static void OpenWindow()
        {
            _window = GetWindow<VersionPopUpWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(270, 200);
            _window.titleContent = new GUIContent("Version Pop-Up window", EditorIcons.RulerRect.Active);
            _window.Show();
        }

        [OnInspectorGUI]
        private void CreateLabel()
        {
            EditorGUILayout.LabelField("Want to create a new Version file?", SirenixGUIStyles.BoldLabelCentered);
            GUILayout.Space(50);
        }

        [ButtonGroup("Group"), LabelText("Yes"), GUIColor(0, 1, 0)]
        private void ButtonYes()
        {
            VersionManager.Instance.CreateNewVersionFile();
            GetWindow<VersionPopUpWindow>().Close();
        }
        [ButtonGroup("Group"), LabelText("No"), GUIColor(1, 0, 0)]
        private void ButtonNo()
        {
            UnityEngine.Debug.LogError("<b><color=red> File not created because it was canceled. </color></b>");
            GetWindow<VersionPopUpWindow>().Close();
        }

    }
}

#endif
