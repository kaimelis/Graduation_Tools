#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Custom.Tool.AutoBuild;

namespace Custom.Tool
{
    public class PasswordPopUpWindow : APopUpWindow
    {
        private string _keystorePassword;
        private string _keyAliasPassword;
        private static bool hasKeystore = true;

        public static void OpenWindow()
        {
#if UNITY_2019
            hasKeystore = PlayerSettings.Android.useCustomKeystore;
#else
            hasKeystore = true;
            if (PlayerSettings.Android.keystoreName.Contains("debug") || PlayerSettings.Android.keystoreName == "")
                hasKeystore = false;
#endif
            PasswordPopUpWindow _window = GetWindow<PasswordPopUpWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(400, 200);
            _window.titleContent = new GUIContent("Password Pop Up window", EditorIcons.CharGraph.Active);
            _window.Show();
        }

        [OnInspectorGUI]
        private void CreateLabel()
        {
            if (!hasKeystore)
            {
                GUIStyle s = new GUIStyle(SirenixGUIStyles.BoldTitleCentered);
                s.normal.textColor = Color.red;
                s.fontSize = 18;
                EditorGUILayout.LabelField("Keystore is not present.", s);
                GUILayout.Space(30);
                return;
            }
            EditorGUILayout.LabelField("Type in passwords", SirenixGUIStyles.BoldLabelCentered);
            GUILayout.Space(30);
            _keystorePassword = EditorGUILayout.PasswordField("Key Store Password",_keystorePassword);
            EditorGUILayout.LabelField(_keystorePassword, SirenixGUIStyles.CenteredGreyMiniLabel);
            _keyAliasPassword = EditorGUILayout.PasswordField("Key Alias Password", _keyAliasPassword);
            EditorGUILayout.LabelField(_keyAliasPassword, SirenixGUIStyles.CenteredGreyMiniLabel);
            GUILayout.Space(10);
        }

        [ButtonGroup("Group"), LabelText("Save"), GUIColor(0, 1, 0)]
        private void Save()
        {
            if (!hasKeystore)
                return;
            PasswordManager.SavePassword(_keystorePassword, "KEYSTORE_PASSWORD");
            PasswordManager.SavePassword(_keyAliasPassword, "ALIAS_PASSWORD");
            ParameterManager.Instance.UpdateAndroidPasswords(_keystorePassword, _keyAliasPassword);
            _keyAliasPassword = "";
            _keystorePassword = "";           
            GetWindow<PasswordPopUpWindow>().Close();
        }

        [ButtonGroup("Group"), LabelText("Cancel"), GUIColor(1, 0, 0)]
        private void Cancel()
        {
            _keyAliasPassword = "";
            _keystorePassword = "";
            GetWindow<PasswordPopUpWindow>().Close();
            GetWindow<AutoBuildEditorWindow>().Close();
        }
    }
}
#endif