#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Custom.Tool.AutoBuild
{ 
    public class AndroidParameter : Parameter
    {
        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public int BundleVersionCode;

        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public bool SplitApplicationBinary;

        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public string PackageName;

        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public AndroidBuildSystem BuildSystem;

        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public AndroidArchitecture AndroidArchitecture;

        [BoxGroup("Android")]
        [OnValueChanged("OnValueChange")]
        public ScriptingImplementation ScriptingBackend;

        public AndroidParameter ()
        {
            OnStartSetup();
            ParameterManager.Instance.RegisterParameter(this);
        }

        private void OnStartSetup()
        {
            BundleVersionCode = PlayerSettings.Android.bundleVersionCode;
            PackageName = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
            ScriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);

            BuildSystem = EditorUserBuildSettings.androidBuildSystem;
            AndroidArchitecture = PlayerSettings.Android.targetArchitectures;
            SetSettings();

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
                return;

#if UNITY_2019

            if ((PlayerSettings.Android.keyaliasPass == "" || PlayerSettings.Android.keystorePass == "" ) && PlayerSettings.Android.useCustomKeystore == true)
            {
                string pass = PasswordManager.GetPassword("ALIAS_PASSWORD");
                PlayerSettings.Android.keystorePass = pass;
                string alias = PasswordManager.GetPassword("KEYSTORE_PASSWORD");
                PlayerSettings.Android.keyaliasPass = alias;
                Debug.Log("<b><color=red> Password has been updated.</color></b>");
            }
            else if(PlayerSettings.Android.useCustomKeystore == false)
            {
                Debug.Log("Custom key false");
                PasswordManager.GetPassword("");
            }
#else
            if ((PlayerSettings.Android.keyaliasPass == "" || PlayerSettings.Android.keystorePass == "") && (!PlayerSettings.Android.keyaliasName.Contains("debug") || PlayerSettings.Android.keyaliasName != ""))
            {
                string pass = PasswordManager.GetPassword("ALIAS_PASSWORD");
                PlayerSettings.Android.keystorePass = pass;
                string alias = PasswordManager.GetPassword("KEYSTORE_PASSWORD");
                PlayerSettings.Android.keyaliasPass = alias;
                Debug.Log("<b><color=red> Password has been updated.</color></b>");
            }
            else if (PlayerSettings.Android.keyaliasName.Contains("debug") || PlayerSettings.Android.keyaliasName == "")
            {
                Debug.Log("Custom key false");
                PasswordManager.GetPassword("");
            }
#endif
        }

        public void UpdatePassword(string key, string alias)
        {
            PlayerSettings.Android.keystorePass = key;
            PlayerSettings.Android.keyaliasPass = alias;
        }

        public override void SetSettings()
        {
            base.SetSettings();
            PlayerSettings.Android.bundleVersionCode = BundleVersionCode;
        }

        public override void PrepareSettings()
        {
            base.PrepareSettings();
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                BundleVersionCode += 1;
            PlayerSettings.Android.bundleVersionCode = BundleVersionCode;
            Debug.Log("<b><color=red> Bundle version has been updated.</color></b>");
        }

        protected override void OnValueChange()
        {
            base.OnValueChange();
            
            if (DevelopmentBuild)
            {
                SplitApplicationBinary = false;
            }
            SetupVariables();
        }

        private void SetupVariables()
        {
            //DEVELOPMENT
            PlayerSettings.Android.bundleVersionCode = BundleVersionCode;
            PlayerSettings.Android.useAPKExpansionFiles = SplitApplicationBinary;
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingBackend);
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PackageName);
            EditorUserBuildSettings.androidBuildSystem = BuildSystem;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture;
        }
    }
}
#endif
