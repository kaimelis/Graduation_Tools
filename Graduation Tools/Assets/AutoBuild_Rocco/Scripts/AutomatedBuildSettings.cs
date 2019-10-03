#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Custom.Tool
{
    [CreateAssetMenu(fileName = "AutomatedBuildSettings", menuName = "Tools/AutomatedBuildSettings")]
    public class AutomatedBuildSettings : SerializedScriptableObject
    {
        [ShowInInspector]
        public Dictionary<SceneAsset, bool> SceneList = new Dictionary<SceneAsset, bool>();
    }
}

#endif
