#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Custom.Tool.AutoBuild
{
    public class BuildManager
    {
        private static BuildManager _instance;
        public static BuildManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new BuildManager();
                return _instance;
            }
            private set { _instance = new BuildManager(); }
        }

        public void Build()
        {
            BuildPath = Directory.GetCurrentDirectory() + "/Builds/";
            if (!EditorUserBuildSettings.development)
                 BuildName = VersionManager.Instance.GetVersion();
            else
                BuildName = VersionManager.Instance.GetVersion(true);

            BuildPopUpWindow.OpenWindow();
        }

        public void MakeABuild()
        {
            //update version if needed
            if (!VersionManager.Instance.CheckVersionUpdate())
            {
                Debug.LogError("<b><color=red>There is no version.!</color></b>");
                return;
            }


            BuildPath += BuildName + "/";

            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
                BuildName += ".apk";
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
                BuildName += "";
            else
                BuildName += ".exe";

            BuildReport build = BuildPipeline.BuildPlayer(GetScenePaths(), BuildPath + BuildName, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            if (File.Exists(BuildPath + BuildName) && build)
            {
                //check if windows and not development
                if(!EditorUserBuildSettings.development && (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows))
                   GitHande.RunGitCommand("tbs unity production");
                //GitHande.RunGitCommand("/c/Users/kaime/Documents/00_MOKSLAI/Graduation/TBS/tbs/tbs unity production");
                if (!FileReaderWriter.FailGitSafeRead())
                {

                    Debug.LogError("<b><color=red> Build has was made, but git failed. </color></b>");
                    return;
                }

                Debug.Log("<b><color=green> Build has been sucesfully made </color></b>");
                return;
            }
            else
                Debug.LogError("<b><color=red> Build failed. Is your repo clean? </color></b>");

        }
        public string BuildPath { get; set; } = Directory.GetCurrentDirectory() + "/Builds/";

        public string BuildName { get; set; } = "unknown";
        public string NameBuildNoExt { get; set; } = "";

        public void SwitchPlatform(string platform)
        {
            if(platform.Contains("Android"))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android,BuildTarget.Android);
            }
            else if(platform.Contains("iOS"))
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS);
            }
            else
            {
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
            }
        }

        private string[] GetScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            return scenes;
        }
    }
}

#endif
