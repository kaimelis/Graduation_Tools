#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace Custom.Tool
{
    public enum BuildType
    {
        Left,
        Center,
        Right
    }

    public class BuildData
    {
        public string screen;
        public string ip;
        public int port;
        public int waitingTime;
        public bool deviceAuthentication;
        public string username;
    }

    public class AutomatedBuildEditor : OdinEditorWindow
    {
        [SerializeField]
        public AutomatedBuildSettings buildSettings;

        private Dictionary<SceneAsset, bool> _settingsDictionary = new Dictionary<SceneAsset, bool>();
        private int _buildNumber = 0;
        private int _finishedBuilds = 0;
        private string _currentName;
        private string _buildPath;
        private BuildType _buildType;

        [MenuItem("CustomTools/AutoBuild", priority = 1)]
        public static void ShowWindow()
        {
            GetWindow<AutomatedBuildEditor>().Show();
        }

        [ShowInInspector]
        public Dictionary<SceneAsset, bool> SceneSettings
        {
            get
            {
                if (buildSettings == null)
                    return new Dictionary<SceneAsset, bool>();
                _settingsDictionary = new Dictionary<SceneAsset, bool>();
                _settingsDictionary = buildSettings.SceneList;
                return _settingsDictionary;
            }
            set
            {
            }
        }

        [ShowInInspector]
        public string Version
        {
            get{ return PlayerSettings.bundleVersion; }
            set { PlayerSettings.bundleVersion = value; }
        }

        [Button]
        private void Build()
        {
            _buildNumber = 0;
            _finishedBuilds = 0;

            for (int i = 0; i < _settingsDictionary.Count; i++)
            {
                if(_settingsDictionary.Values.ElementAt(i))
                {
                    _buildNumber++;
                }
            }
            Debug.Log("In total there will be " + _buildNumber + " builds.");
            if(_buildNumber == 0)
            {
                Debug.Log("No scenes are selected. Can't make a build");
                return;
            }
             MakeABuild();
        }

        private void SetupBuildSettings()
        {
            if (_buildNumber <= _finishedBuilds)
                return;
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
            SceneAsset currentScene = _settingsDictionary.Keys.ElementAt(_finishedBuilds);
            string scenePath = AssetDatabase.GetAssetPath(currentScene);
            if (!string.IsNullOrEmpty(scenePath))
            {
                EditorBuildSettingsScene item = new EditorBuildSettingsScene(scenePath, _settingsDictionary.Values.ElementAt(_finishedBuilds));
                if (!editorBuildSettingsScenes.Contains(item))
                {
                    editorBuildSettingsScenes.Add(item);
                    if(currentScene.name.Contains("center") || currentScene.name.Contains("Center") || currentScene.name.Contains("Central"))
                    {
                        _buildType = BuildType.Center;
                    }
                    else if (currentScene.name.Contains("right") || currentScene.name.Contains("Right"))
                    {
                        _buildType = BuildType.Right;
                    }
                    else if (currentScene.name.Contains("left") || currentScene.name.Contains("Left"))
                    {
                        _buildType = BuildType.Left;
                    }
                }
                else
                    return;
            }
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
            editorBuildSettingsScenes.Clear();
        }

        private void MakeABuild()
        {
            SetupBuildSettings();
            if (_buildType == BuildType.Center)
            {
                _currentName = PlayerSettings.productName + "_Center";
            }
            else if (_buildType == BuildType.Right)
            {
                _currentName = PlayerSettings.productName + "_Right";
            }
            else
            {
                _currentName = PlayerSettings.productName + "_Left";
            }
            string buildName = _currentName + @"\" +_currentName;
            string buildPath;
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows)
                buildPath = Directory.GetCurrentDirectory() + @"\Builds\" + buildName + ".exe";
            else
                buildPath = Directory.GetCurrentDirectory() + @"\Builds\" + buildName;
            Debug.Log("Build was made for " + _currentName);
            _buildPath = Directory.GetCurrentDirectory() + @"\Builds\" + _currentName;
            ChangeJsonFile();
            BuildReport build = BuildPipeline.BuildPlayer(GetScenePaths(), buildPath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            if (build)
            {
                _finishedBuilds++;
                Debug.Log("Finished making a build " + _finishedBuilds);
                DeleteStreamingAssets();
                
                if (_buildNumber <= _finishedBuilds)
                {
                    Debug.Log("Made builds for all the scenes.Done");
                    return;
                }
                MakeABuild();
            }
            return;
        }
        
        /// <summary>
        /// Deleting files after build so that build would not have unecessary files. 
        /// </summary>
        private void DeleteStreamingAssets()
        {
            Debug.Log("<color=red>Deleting streaming assets</color>");
            string filesToDelete;

            if (_buildType == BuildType.Center)
                filesToDelete = "center*";
            else if (_buildType == BuildType.Right)
                filesToDelete = "right*";
            else
                filesToDelete = "left*";

            int x = 0;
            string path = _buildPath + @"\" + _currentName + @"_Data\" + @"StreamingAssets\";
            if (!Directory.Exists(path))
                return;
            string[] files = Directory.GetFiles(path, filesToDelete, SearchOption.AllDirectories);

            foreach (string f in files)
            {
                //enable if you want to delete this
                //however a way to find all objects need to be found
              //  File.Delete(f);
                x++;
            }
            Debug.Log("Deleted " + x + " files.");
        }

        private void ChangeJsonFile()
        {
            string jsonFile = Application.streamingAssetsPath + "/settings.json";
            string saveString = File.ReadAllText(jsonFile);
            BuildData saveData = JsonUtility.FromJson<BuildData>(saveString);
            if (_buildType == BuildType.Center)
                saveData.screen =  "Center";
            else if (_buildType == BuildType.Right)
                saveData.screen = "Right";
            else
                saveData.screen = "Left";
            string jsonInfo = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(jsonFile,jsonInfo);
           
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