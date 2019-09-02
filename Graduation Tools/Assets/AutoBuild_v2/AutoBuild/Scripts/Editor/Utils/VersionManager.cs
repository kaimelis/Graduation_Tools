#if UNITY_EDITOR
using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Custom.Tool.AutoBuild
{
    public class VersionManager
    {
        #region Instance
        private static VersionManager _instance = null;
        public static VersionManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new VersionManager();
                return _instance;
            }
            private set
            {
                _instance = new VersionManager();
            }
        }
        #endregion
        private string _version;
        private string _versionSuggestion;
        private string _actualVersion;
        private string _bundleCode;
        private string _pathVersion = Directory.GetCurrentDirectory() + "/VERSION";
        private string _pathDevelopVersion = Directory.GetCurrentDirectory() + "/tmp/local_version";
       
        /// <summary>
        /// When a button update version is clicked this function is called that upgrades the version.
        /// </summary>
        public void UpdateVersion()
        {
            if(!EditorUserBuildSettings.development && (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS || EditorUserBuildSettings.activeBuildTarget != BuildTarget.StandaloneOSX))
            {
                ///c/Users/kaime/Documents/00_MOKSLAI/Graduation/TBS/tbs/tbs unity prepare
                GitHande.RunGitCommand("tbs unity prepare");

                if (!FileReaderWriter.FailGitSafeRead())
                    return;
            }

            //check if we have version file
            if(!EditorUserBuildSettings.development)
            {
                if (FileReaderWriter.CheckIfFileExists(_pathVersion))
                {
                    UpgradeVersionPopWindow.OpenWindow();
                }
                else
                {
                    VersionPopUpWindow.OpenWindow();
                    Debug.LogError("Version file doesn't exists. Please create a new one.");
                }
            }
            else
                Debug.LogError("Sorry you are in develop. Can't upgrade the version.");
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CheckVersionUpdate()
        {
            if (!EditorUserBuildSettings.development)
            {
                if(CheckVersionFile() != "")
                {
                    if (PlayerSettings.bundleVersion == CheckVersionFile())
                        return true;
                    if(EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                        FileReaderWriter.WriteToFile(_pathVersion, PlayerSettings.bundleVersion);
                    return true;
                }
                return false;
            }
            return true;

        }

        /// <summary>
        /// Creates a VERSION file. If repository does not have a tag then it will create a tag and a file.
        /// </summary>
        public void CreateNewVersionFile()
        {
            //GitHande.RunGitCommand("/c/Users/kaime/Documents/00_MOKSLAI/Graduation/TBS/tbs/tbs unity version v0.1.0");
            GitHande.RunGitCommand("tbs unity version v0.1.0b1");

            if (!FileReaderWriter.FailGitSafeRead())
                return;

            if(!FileReaderWriter.CheckIfFileExists(_pathVersion))
            {
                Debug.Log("<b><color=red> File was not created. Is your repo clean?</color></b>");
                return;
            }
            Debug.Log("<b><color=green> File was created.</color></b>");

            //check the version of file and compare to current in unity
            if(!EditorUserBuildSettings.development)
                UpgradeVersionPopWindow.OpenWindow();

        }

        public string GetSuggestionVersion()
        {
            string suggestionVersion = GetVersion();
            _versionSuggestion = SplitVersion(suggestionVersion);
            return _versionSuggestion;
        }

        public string GetVersion(bool development = false)
        {
            if(development)
            {
                _version = GetVersionFromFile(true);
            }
            else
            {
                _version = PlayerSettings.bundleVersion;
                if (!_version.Contains("v") && _version != "")
                    _version = GetVersionFromFile();
                string fileVersion = GetVersionFromFile();
                if (_version != fileVersion)
                    _version = GetHigherVersion(PlayerSettings.bundleVersion, fileVersion);
            }

            return _version;
        }

        public void SetVersion(string version)
        {
            _version = version;
            PlayerSettings.bundleVersion = _version;

            Debug.Log("<b><color=Green> Version set to be : </color></b>" + _version);
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                FileReaderWriter.WriteToFile(_pathVersion, _version);
            else
                FileReaderWriter.WriteToFile(_pathVersion, _actualVersion);


            ParameterManager.Instance.PrepareSettings();
        }

        public string GetBundleCode()
        {
            if(_bundleCode == null)
            {
                string version = CheckVersionFile();
                var splitMajor = version.Split('.');
                var splitBuild = splitMajor[2].Split('b');
                if (splitBuild.Length > 1)
                    _bundleCode = splitBuild[1];
                else
                    _bundleCode = "1";
                return _bundleCode;
            }
             return _bundleCode;
        }

        private string GetHigherVersion(string version1, string version2)
        {
            if (version1 == "" && version2 != "")
                return version2;
            else if (version1 != "" && version2 == "")
                return version1;

            Debug.Log(version1);
            Debug.Log(version2);

            #region version1
            var splitVersion1Dot = version1.Split('.');
            var splitVersion1Major = splitVersion1Dot[0].Split('v');
            string majorVersion1 = "";
            if (splitVersion1Major.Length > 1)
                 majorVersion1 = splitVersion1Major[1];
            else
                 majorVersion1 = splitVersion1Major[0];
            string minorVersion1 = splitVersion1Dot[1];
            var splitBuildVersion1 = splitVersion1Dot[2].Split('b');
            string buildVersion1 = splitBuildVersion1[0];
            string buildVersion1Beta = "";
            if (splitBuildVersion1.Length > 1)
                buildVersion1Beta = splitBuildVersion1[1];
            #endregion

            #region version2
            var splitVersion2Dot = version2.Split('.');
            var splitVersion2Major = splitVersion2Dot[0].Split('v');
            string majorVersion2 = splitVersion2Major[1];
            string minorVersion2 = splitVersion2Dot[1];
            var splitBuildVersion2 = splitVersion2Dot[2].Split('b');
            string buildVersion2 = splitBuildVersion2[0];
            string buildVersion2Beta = "";
            if (splitBuildVersion2.Length > 1)
                buildVersion2Beta = splitBuildVersion2[1];
            #endregion

            if (Convert.ToInt32(majorVersion1) > Convert.ToInt32(majorVersion2))
            {
                return version1;
            }
            else if (Convert.ToInt32(minorVersion1) > Convert.ToInt32(minorVersion2))
            {
                return version1;
            }
            else if (Convert.ToInt32(buildVersion1) > Convert.ToInt32(buildVersion2))
            {
                return version1;
            }
            else if (buildVersion1Beta != "" && buildVersion2Beta != "")
            {
                if (Convert.ToInt32(buildVersion1Beta) > Convert.ToInt32(buildVersion2Beta))
                    return version1;
                else
                    return version2;
            }
            else
                return version2;
        }

        private string SplitVersion(string version)
        {
            Debug.Log(version);
            if (version == "" || !version.Contains("v"))
                version = GetVersionFromFile();

            Debug.Log("<b><color=blue> Current version is: </color></b> = " + version);
            //v0 1 0
            //v0.1.0
            //v0 1 0b1
            var splitVersionDot = version.Split('.');
            var splitMajorV = splitVersionDot[0].Split('v');
            int buildVersion;
            int buildVersionMinor;
            var splitBuildB = splitVersionDot[2].Split('b');
            //need to check if there is b to split from or not  
            if (splitBuildB.Length > 1)
            {
                if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
                {
                    buildVersionMinor = Convert.ToInt32(splitBuildB[1]) + 1;
                    buildVersion = Convert.ToInt32(splitBuildB[0]);
                }
                else
                {
                    buildVersion = Convert.ToInt32(splitBuildB[0]) + 1;
                    buildVersionMinor = 1;
                }
                
                version = "v" + splitMajorV[1] + "." + splitVersionDot[1] + "." + buildVersion.ToString() + "b" + buildVersionMinor.ToString();
                _bundleCode = buildVersionMinor.ToString();
                _actualVersion = version;
            }
            else
            {
                buildVersion = Convert.ToInt32(splitVersionDot[2]) + 1;
                version = "v" + splitMajorV[1] + "." + splitVersionDot[1] + "." + buildVersion.ToString();
                _bundleCode = "1";
            }
            
            if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
            {

                version = splitMajorV[1] + "." + splitVersionDot[1] + "." + buildVersion.ToString();
            }
            Debug.Log("<b><color=green> Suggested version is: </color></b> = " + version);
            return version;
        }

        private string GetVersionFromFile(bool development = false)
        {
            string fileVersion = "";
            if(EditorUserBuildSettings.development || development)
            {
                //GitHande.RunGitCommand("/c/Users/kaime/Documents/00_MOKSLAI/Graduation/TBS/tbs/tbs unity develop");
                GitHande.RunGitCommand("tbs unity develop");
                if (FileReaderWriter.FailGitSafeRead())
                {
                    fileVersion = FileReaderWriter.ReadLineFromFile(_pathDevelopVersion);
                    Debug.Log("<b><color=blue> Develop version is : </color></b>" + fileVersion);
                }
            }
            else
            {
                if (FileReaderWriter.CheckIfFileExists(_pathVersion))
                {
                    fileVersion = FileReaderWriter.ReadLineFromFile(_pathVersion);
                    //Debug.Log("<b><color=blue> Version is : </color></b>" + fileVersion);
                }
                else
                {
                    Debug.LogError("<b><color=red> Version file does not exists. </color></b>");
                    VersionPopUpWindow.OpenWindow();
                }
            }

            
            //clean up the end of the line
            return fileVersion.Trim();
        }

        private string CheckVersionFile()
        {
            if (FileReaderWriter.CheckIfFileExists(_pathVersion))
            {
                return FileReaderWriter.ReadLineFromFile(_pathVersion);
            }
            return PlayerSettings.bundleVersion;
        }
    }
}
#endif
