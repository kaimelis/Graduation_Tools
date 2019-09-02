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
    public class BuildPopUpWindow : APopUpWindow
    {
        private static BuildPopUpWindow _window;
        private static string _buildName;
        public static void OpenWindow()
        {
            _buildName = BuildManager.Instance.BuildName;
            _window = GetWindow<BuildPopUpWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(600, 200);
            _window.titleContent = new GUIContent("Build popup window", EditorIcons.RulerRect.Active);
            _window.Show();
        }

        [OnInspectorGUI, PropertyOrder(1)]
        private void GUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Create new build.Ready?", SirenixGUIStyles.BoldLabelCentered);

            EditorGUILayout.LabelField("Build will be created in: " + BuildManager.Instance.BuildPath, SirenixGUIStyles.CenteredWhiteMiniLabel);
            EditorGUILayout.LabelField("Build name will be:  " + BuildManager.Instance.BuildName, SirenixGUIStyles.CenteredWhiteMiniLabel);
            EditorGUILayout.LabelField("Build is being made for:  " + EditorUserBuildSettings.activeBuildTarget.ToString(), SirenixGUIStyles.CenteredWhiteMiniLabel);
            GUILayout.Space(5);
        }

        [FolderPath(AbsolutePath = true), PropertyOrder(2), OnValueChanged("UpdatePath")]
        public string FolderPath;

        private void UpdatePath()
        {
            BuildManager.Instance.BuildPath = FolderPath + "/";
        }

        [OnInspectorGUI, PropertyOrder(3)]
        private void FileNameUpdate()
        {
            GUILayout.Space(5);
            string fileName = EditorGUILayout.TextField("File name", _buildName, SirenixGUIStyles.RichTextLabel);
            _buildName = fileName;
            BuildManager.Instance.BuildName = fileName;
            GUILayout.Space(10);
        }

        [ButtonGroup("Group", Order = 4), LabelText("Create"), GUIColor(0, 1, 0)]
        private void Create()
        {
            GetWindow<BuildPopUpWindow>().Close();
            BuildManager.Instance.MakeABuild();
        }

        [ButtonGroup("Group",Order = 4), LabelText("Cancel"), GUIColor(0.9f, 0.2f, 0)]
        private void Cancel()
        {
            GetWindow<BuildPopUpWindow>().Close();
        }

    }
}
#endif
