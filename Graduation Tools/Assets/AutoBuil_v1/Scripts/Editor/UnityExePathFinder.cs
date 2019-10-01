using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace TBS.CustomTools
{
    [InitializeOnLoad]
    public class UnityExePathFinder
    {
        static UnityExePathFinder()
        {
            string unityPath = Environment.GetCommandLineArgs()[0];
            if(File.Exists("tmp/unity_path"))
            {
                if (File.ReadAllText("tmp/unity_path") == unityPath)
                    return;
            }
            else if (!Directory.Exists("tmp"))
            {
                Directory.CreateDirectory("tmp");
            }
            File.WriteAllText("tmp/unity_path", unityPath);
            Debug.Log("tmp/unity_path has been updated with path : " + unityPath);
        }
    }
}
