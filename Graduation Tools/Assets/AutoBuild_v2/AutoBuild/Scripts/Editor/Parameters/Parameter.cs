#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Custom.Tool.AutoBuild
{
    public abstract class Parameter
    {
        /// <summary>
        /// Version of the project that represent a tag in source control
        /// </summary>
        [BoxGroup("General", Order = 3), OnValueChanged("OnValueChange")]
        public string Version;

        /// <summary>
        /// 
        /// </summary>
        [BoxGroup("General", Order = 3), OnValueChanged("OnValueChange")]
        public string CompanyName;

        /// <summary>
        /// 
        /// </summary>
        [BoxGroup("General", Order = 3), OnValueChanged("OnValueChange")]
        public string ProductName;

        [BoxGroup("General/Develop")]
        [OnValueChanged("OnValueChange")]
        public bool DevelopmentBuild;
        [BoxGroup("General/Develop"), ShowIf("DevelopmentBuild")]
        public bool AutoconnectProfiler;
        [BoxGroup("General/Develop"), ShowIf("DevelopmentBuild")]
        public bool ScriptDebugging;
        [BoxGroup("General/Develop"), ShowIf("DevelopmentBuild")]
        public bool ScriptsOnlyBuild;
       
        public virtual void SetSettings()
        {
            Version = PlayerSettings.bundleVersion;
            CompanyName = PlayerSettings.companyName;
            ProductName = PlayerSettings.productName;

            DevelopmentBuild = EditorUserBuildSettings.development;
            AutoconnectProfiler = EditorUserBuildSettings.connectProfiler;
            ScriptDebugging = EditorUserBuildSettings.allowDebugging;
            ScriptsOnlyBuild = EditorUserBuildSettings.buildScriptsOnly;
        }

        public virtual void PrepareSettings()
        {
            Version = PlayerSettings.bundleVersion;

            EditorUserBuildSettings.development = DevelopmentBuild;
            EditorUserBuildSettings.connectProfiler = AutoconnectProfiler;
            EditorUserBuildSettings.allowDebugging = ScriptDebugging;
            EditorUserBuildSettings.buildScriptsOnly = ScriptsOnlyBuild;
        }

        protected virtual void OnValueChange()
        {
            if (!DevelopmentBuild)
            {
                AutoconnectProfiler = false;
                ScriptDebugging = false;
                ScriptsOnlyBuild = false;
            }

            PlayerSettings.bundleVersion = Version;
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;

            EditorUserBuildSettings.development = DevelopmentBuild;
            EditorUserBuildSettings.connectProfiler = AutoconnectProfiler;
            EditorUserBuildSettings.allowDebugging = ScriptDebugging;
            EditorUserBuildSettings.buildScriptsOnly = ScriptsOnlyBuild;

            ParameterManager.Instance.UpdateGeneralSettings();
        }
    }    
}

#endif
