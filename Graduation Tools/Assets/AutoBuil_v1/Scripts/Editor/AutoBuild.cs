using System;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace TBS.CustomTools
{
    public struct XmlInfo
    {
        public int amountOfTestsRan;
        public int amountOfFailedTests;
    }

    public class AutoBuild : MonoBehaviour
    {

        #region Building
        private static string _xmlText = "";
        private static bool _isDevelop = true;
        private const string _unityResult = "unity_result";
        private const string _unityLog = "unity_log";


        private static void SetDevelopmentBuild()
        {
            _isDevelop = true;
            RunBuild();
        }

        private static void BuildProductionBuild()
        {
            _isDevelop = false;
            SetPasswordForProd();
            RunBuild();
        }


        private static void SetProjectSettingsDev()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {

            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                PlayerSettings.Android.keyaliasName = "";
                PlayerSettings.Android.useAPKExpansionFiles = false;
                WriteToTxt("SUCCESS", _unityResult);
            }
            else
            {
                WriteToTxt("FAIL", _unityResult);
                WriteToTxt("Wrong platform on Unity. Comeback to Unity and and  chage to which you need.", _unityLog);
            }

            EditorUserBuildSettings.development = true;
            EditorUserBuildSettings.connectProfiler = true;
            EditorUserBuildSettings.allowDebugging = true;

        }

        private static void SetProjectSettingsProd()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                //PlayerSettings.iOS.buildNumber += 1;
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                PlayerSettings.bundleVersion = GetCurrentVersionProd();
                PlayerSettings.Android.bundleVersionCode += 1;
                WriteToTxt("SUCCESS", _unityResult);
            }
            else
            {
                WriteToTxt("FAIL", _unityResult);
                WriteToTxt("Wrong platform on Unity. Comeback to Unity and and  chage to which you need.", _unityLog);
                return;
            }


        }

        private static void SetPasswordForProd()
        {
            string keyPassword = "";
            StreamReader reader = new StreamReader(GetProjectPath() + "/tmp/key_password");
            keyPassword = reader.ReadLine();
            reader.Close();

            string aliasPassword = "";
            StreamReader reader2 = new StreamReader(GetProjectPath() + "/tmp/alias_password");
            aliasPassword = reader2.ReadLine();
            reader2.Close();


            PlayerSettings.Android.keystorePass = keyPassword;
            // PlayerSettings.Android.keyaliasName = "test";
            PlayerSettings.Android.keyaliasPass = aliasPassword;
            PlayerSettings.Android.useAPKExpansionFiles = true;
            

            EditorUserBuildSettings.development = false;
            EditorUserBuildSettings.connectProfiler = false;
            EditorUserBuildSettings.allowDebugging = false;
        }

        private static void RunBuild()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {

            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                string name = "";
                BuildReport build = null;
                if (!_isDevelop)
                    name = "Builds/" + GetCurrentVersionProd() + ".apk";
                else
                    name = "Builds/" + GetCurrentVersionDev() + ".apk";

                WriteToTxt("FAIL", _unityResult);
                build = BuildPipeline.BuildPlayer(GetScenePaths(), name, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);


                WriteToTxt(build ? "SUCCESS" : "FAIL", _unityResult);

                _xmlText = build ? "Successfully made a build " : "Build has failed ";
                WriteToTxt(_xmlText, _unityLog);

            }
            else
            {
                WriteToTxt("Wrong platform on Unity. Comeback to Unity and and  chage to which you need.", _unityLog);
                return;
            }

        }

        private static string GetCurrentVersionProd()
        {
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {
                return "";
            }
            else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                //READING VERSION FILE
                string version = "";
                StreamReader reader = new StreamReader("VERSION");
                version = reader.ReadLine();
                reader.Close();
                return version;
            }
            else
            {
                WriteToTxt("Wrong platform on Unity. Comeback to Unity and and  chage to which you need.", _unityLog);
                return "0.0.0";
            }

        }

        private static string GetCurrentVersionDev()
        {
            string version = "";
            StreamReader reader = new StreamReader(GetProjectPath() + "/tmp/local_version");
            version = reader.ReadLine();
            reader.Close();
            return version;
        }

        private static void RunTests()
        {
            SetInformationXml();
        }
#endregion

#region XmlParsing
        private static XmlDocument _xmlDoc;
        private static XmlInfo _xmlInfo;
        private static string _filename;

        private static void SetInformationXml()
        {
            //_filename = "*.txt";
            string[] filePaths = filePaths = Directory.GetFiles(GetProjectPath(), "*.xml");
            if (filePaths.Length > 0)
            {
                filePaths = Directory.GetFiles(GetProjectPath(), "*.xml");
                _filename = Path.GetFileName(filePaths[0]);
                if (File.Exists(_filename))
                {
                    _xmlInfo = new XmlInfo();
                    LoadXmlFromFile();
                }
                else
                {
                    string text = "Have not found xml file for tests \n" +
                                    "Script AutoBuild or tbs-build has errors \n" +
                                    "Ask Kotryna for help";
                    WriteToTxt(text, _unityLog);
                }
            }
            else
            {
                string text = "Have not found xml file for tests \n" +
                                "Script AutoBuild or tbs-build has errors \n" +
                                "Ask Kotryna for help";
                WriteToTxt(text, _unityLog);

            }
        }
        private static void LoadXmlFromFile()
        {
            _xmlDoc = new XmlDocument();
            _xmlDoc.LoadXml(System.IO.File.ReadAllText(_filename));
            if (!_isDevelop)
                ReadXmlForBuild();
            else
                ReadXmlFromFile();
        }
        private static void ReadXmlFromFile()
        {
            if (_xmlDoc == null)
                return;
            foreach (XmlElement element in _xmlDoc.SelectNodes("test-run"))
            {
                XmlInfo info = new XmlInfo();
                info.amountOfFailedTests = int.Parse(element.GetAttribute("failed"));
                info.amountOfTestsRan = int.Parse(element.GetAttribute("testcasecount"));
                _xmlInfo = info;
            }

            WriteXmlDataToTxt();
        }
        private static void WriteXmlDataToTxt()
        {

            _xmlText = "Tests ran in total = " + _xmlInfo.amountOfTestsRan + "\n" +
                    "------------------- \n";
            if (_xmlInfo.amountOfFailedTests > 0)
            {
                _xmlText += "Tests that failed = " + _xmlInfo.amountOfFailedTests + "\n" +
                        "Go back to Unity and fix the errors \n \n";

                WriteToTxt(_xmlText, _unityLog);
                DeletePreviousXMLFile();
                WriteToTxt("FAIL", _unityResult);
            }
            else
            {
                _xmlText += "Tests Has Succeeded !!!!!!!!!!!!!!! \n \n";
                WriteToTxt(_xmlText, _unityLog);
                WriteToTxt("SUCCESS", _unityResult);
            }
        }

        private static void ReadXmlForBuild()
        {
            foreach (XmlElement element in _xmlDoc.SelectNodes("test-run"))
            {
                XmlInfo info = new XmlInfo();
                info.amountOfFailedTests = int.Parse(element.GetAttribute("failed"));
                info.amountOfTestsRan = int.Parse(element.GetAttribute("testcasecount"));
                _xmlInfo = info;
            }
            if (_xmlInfo.amountOfFailedTests > 0)
            {
                WriteToTxt("FAIL", _unityResult);
            }
            else
            {
                WriteToTxt("SUCCESS", _unityResult);
            }

            DeletePreviousXMLFile();
        }
        private static void DeletePreviousXMLFile()
        {
            string[] filePaths = Directory.GetFiles(GetProjectPath(), "*.xml");
            File.Delete(Path.GetFileName(filePaths[0]));
            RemoveAt(ref filePaths, 0);
        }

        private static void WriteToTxt(string text, string fileName)
        {
            if (File.Exists(GetProjectPath() + "/tmp/" + fileName))
            {
                File.WriteAllText("tmp/" + fileName, text);
            }
        }
#endregion

#region General
        private static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                // moving elements downwards, to fill the gap at [index]
                arr[a] = arr[a + 1];
            }
            // finally, let's decrement Array's size by one
            Array.Resize(ref arr, arr.Length - 1);
        }

        private static string GetProjectPath()
        {
            string path = Path.GetDirectoryName(Application.dataPath);

            return path;
        }

        private static string GetProjectName()
        {
            string[] s = Application.dataPath.Split('/');
            return s[s.Length - 2];
        }

        private static string[] GetScenePaths()
        {
            string[] scenes = new string[EditorBuildSettings.scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }

            return scenes;
        }
#endregion

    }
}
