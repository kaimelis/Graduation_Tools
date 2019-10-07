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

    public struct SceneInfo
    {
       public bool BuildRight;
       public bool BuildLeft;
       public bool BuildCenter;
    }

    public class AutomatedBuildEditor : OdinEditorWindow
    {
        public SceneInfo SceneBuildType;

        [ReadOnly]
        public SceneAsset rightScene;
        [ReadOnly]
        public SceneAsset leftScene;
        [ReadOnly]
        public SceneAsset centerScene;
        [ReadOnly]
        public List<SceneAsset> sceneList = new List<SceneAsset>();
        private List<EditorBuildSettingsScene> editorBuilds = new List<EditorBuildSettingsScene>();
        private string _buildPath;
        private BuildType _buildType;
        private int _buildCount = 0;
        private int _currentBuild = 0;
      
        [MenuItem("CustomTools/AutoBuild", priority = 1)]
        public static void ShowWindow()
        {
            GetWindow<AutomatedBuildEditor>().Show();
        }

        [Button]
        private void Build()
        {
            sceneList.Clear();
            editorBuilds.Clear();
            _buildCount = 0;
            _currentBuild = 0;

            if(SceneBuildType.BuildLeft && SceneBuildType.BuildRight && SceneBuildType.BuildCenter)
            {
                _buildCount = 3;
            }
            else if(atLeastTwo(SceneBuildType.BuildLeft, SceneBuildType.BuildRight, SceneBuildType.BuildCenter))
            {
                _buildCount = 2;
            }
            else if(atLeastOne(SceneBuildType.BuildLeft, SceneBuildType.BuildRight, SceneBuildType.BuildCenter))
            {
                _buildCount = 1;
            }
            else
            {
                _buildCount = 0;
                Debug.LogError("Sorry no scene was chosen to make a build.");
                return;
            }


            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
               // Debug.Log(EditorBuildSettings.scenes[i].path);
                //add to the list the scenes
                sceneList.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[i].path));
            }

            for (int i = 0; i < sceneList.Count; i++)
            {
                if(sceneList[i].name.Contains("right") || sceneList[i].name.Contains("Right"))
                {
                    rightScene = sceneList[i];
                    EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
                    scene.path = AssetDatabase.GetAssetPath(sceneList[i]);
                    scene.enabled = false;
                    editorBuilds.Add(scene);

                }
                else if (sceneList[i].name.Contains("left") || sceneList[i].name.Contains("Left"))
                {
                    leftScene = sceneList[i];
                    EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
                    scene.path = AssetDatabase.GetAssetPath(sceneList[i]);
                    scene.enabled = false;
                    editorBuilds.Add(scene);
                }
                else if (sceneList[i].name.Contains("center") || sceneList[i].name.Contains("Center"))
                {
                    centerScene = sceneList[i];
                    EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
                    scene.path = AssetDatabase.GetAssetPath(sceneList[i]);
                    scene.enabled = false;
                    editorBuilds.Add(scene);
                }
                else
                {
                    EditorBuildSettingsScene scene = new EditorBuildSettingsScene();
                    scene.path = AssetDatabase.GetAssetPath(sceneList[i]);
                    scene.enabled = true;
                    editorBuilds.Add(scene);
                }
            }
            DisableScenes();
            MakeBuild();
        }

        private void MakeBuild()
        {
            //change the enabled shit
            DisableScenes();
            for (int i = 0; i < editorBuilds.Count; i++)
            {
                if (SceneBuildType.BuildLeft && (editorBuilds[i].path.Contains("left") || editorBuilds[i].path.Contains("Left")))
                {
                    editorBuilds[i].enabled = true;
                    _buildType = BuildType.Left;
                    SceneBuildType.BuildLeft = false;
                    Debug.Log("Prepared to make a build for left screen " + editorBuilds[i].path);
                    break;
                }
                else if (SceneBuildType.BuildRight && (editorBuilds[i].path.Contains("right") || editorBuilds[i].path.Contains("Right")))
                {
                    editorBuilds[i].enabled = true;
                    _buildType = BuildType.Right;
                    SceneBuildType.BuildRight = false;
                    Debug.Log("Prepared to make a build for right screen " + editorBuilds[i].path);
                    break;
                }
                else if (SceneBuildType.BuildCenter && (editorBuilds[i].path.Contains("center") || editorBuilds[i].path.Contains("Center")))
                {
                    editorBuilds[i].enabled = true;
                    _buildType = BuildType.Center;
                    SceneBuildType.BuildCenter = false;
                    Debug.Log("Prepared to make a build for center screen " + editorBuilds[i].path);
                    break;
                }
                else if(rightScene == null || leftScene == null || centerScene == null)
                {
                    if (SceneBuildType.BuildLeft)
                    {
                        _buildType = BuildType.Left;
                        SceneBuildType.BuildLeft = false;
                    }
                    else if (SceneBuildType.BuildRight)
                    {
                        _buildType = BuildType.Right;
                        SceneBuildType.BuildRight = false;
                    }
                    else if (SceneBuildType.BuildCenter)
                    {
                        _buildType = BuildType.Center;
                        SceneBuildType.BuildCenter = false;
                    }
                    else
                        return;
                    break;
                }
            }

            _currentBuild++;
            EditorBuildSettings.scenes = editorBuilds.ToArray();
            List<string> scenesPaths = new List<string>();

            _buildPath = Directory.GetCurrentDirectory() + "/Builds/" + PlayerSettings.productName + "_" + _buildType.ToString();
            string buildPath = _buildPath + "/" + PlayerSettings.productName + "_" + _buildType.ToString() + ".exe";

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].enabled)
                    scenesPaths.Add((EditorBuildSettings.scenes[i].path));
            }
            ChangeJsonFile();
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            {
                buildOptions.scenes = scenesPaths.ToArray();
                buildOptions.locationPathName = buildPath;
                buildOptions.target = BuildTarget.StandaloneWindows;
                buildOptions.options = BuildOptions.None;
            }
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Failed)
                return;
            scenesPaths.Clear();
            Debug.Log("Made a build for " + _buildType.ToString());
            DeleteFiles();
            if(_buildCount > _currentBuild)
            {
                Debug.Log("Will make a next build");
                MakeBuild();
            }
            else
            {
                Debug.Log("<color=green>Finished making builds</color>");
                return;
            }
        }

        private void DisableScenes()
        {
            for (int i = 0; i < editorBuilds.Count; i++)
            {
                if (editorBuilds[i].path.Contains("left") || editorBuilds[i].path.Contains("Left"))
                {
                    editorBuilds[i].enabled = false;
                }
                else if (editorBuilds[i].path.Contains("right") || editorBuilds[i].path.Contains("Right"))
                {
                    editorBuilds[i].enabled = false;
                }
                else if (editorBuilds[i].path.Contains("center") || editorBuilds[i].path.Contains("Center"))
                {
                    editorBuilds[i].enabled = false;
                }
            }
        }

        private bool atLeastTwo(bool a, bool b, bool c)
        {
            return a ? (b || c) : (b && c);
        }

        private bool atLeastOne(bool a, bool b, bool c)
        {
            if (a && !b && !c)
                return true;
            else if (!a && b && !c)
                return true;
            else if (!a && !b && c)
                return true;
            else
                return false;
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

        private void DeleteFiles()
        {
            Debug.Log("<color=red>Deleting files</color>");
            string newPath = _buildPath +  "/" + PlayerSettings.productName + "_" + _buildType.ToString() + "_Data/StreamingAssets";
            if (_buildType == BuildType.Left || _buildType == BuildType.Right)
            {
               var files =  Directory.GetFiles(newPath, "*center*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Log("<color=red>" + files[i] + "</color>");
                    File.Delete(files[i]);
                }
            }
            if (_buildType == BuildType.Center || _buildType == BuildType.Left)
            {
                var files = Directory.GetFiles(newPath, "*right*",SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Log("<color=red>" + files[i] + "</color>");
                    File.Delete(files[i]);
                }
            }
             if (_buildType == BuildType.Right || _buildType == BuildType.Center)
            {
                var files = Directory.GetFiles(newPath, "*left*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    Debug.Log("<color=red>" + files[i] + "</color>");
                    File.Delete(files[i]);
                }
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

        private string GetProjectFolderPath()
        {
            var s = Application.dataPath;
            s = s.Substring(s.Length - 7, 7); // remove "Assets/"
            return s;
        }
    }
}
#endif