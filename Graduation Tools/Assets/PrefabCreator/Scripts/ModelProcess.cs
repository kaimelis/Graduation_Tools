#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector.Editor;

public class ModelProcess : OdinEditorWindow
{

    [MenuItem("TBS/Tools/Model Process", priority = 0)]
    public static void ShowWindow()
    {
        GetWindow<ModelProcess>().Show();
    }

    [TabGroup("MAIN", "MODELS")]
    [AssetList(CustomFilterMethod = "IsModel", Path = "/Models/", AutoPopulate = true)]
    [InlineEditor(InlineEditorModes.SmallPreview)]
    public List<Object> Models = new List<Object>();

    private bool IsModel(Object obj)
    {
        return PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Model;
    }

    [TabGroup("MAIN", "MODELS")]
    [Button("FBX->Prefab - All Selected Models")]
    public void ProcessModels()
    {
        foreach (GameObject m in Models)
        {
            ProcessModel(m);
        }
    }

    private void ProcessModel(GameObject m)
    {
        string assetPath = AssetDatabase.GetAssetPath(m);
        string folderPath = Path.GetDirectoryName(assetPath) + @"\";

        string fileNmae = Path.GetFileNameWithoutExtension(assetPath);
        string destinyPath = folderPath + fileNmae + ".prefab";
        if (AssetDatabase.LoadAssetAtPath(destinyPath, typeof(GameObject)))
        {
            if (EditorUtility.DisplayDialog("Overwrite Prefab",
                "A prefab " + fileNmae + " already exists at " + destinyPath + ".\nDo you want to overwite it?",
                "Yes",
                "No"))
            {
                GameObject go = AssetDatabase.LoadAssetAtPath(destinyPath, typeof(GameObject)) as GameObject;
                GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(go);
                GameObject prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(objSource, destinyPath, InteractionMode.AutomatedAction);
                DestroyImmediate(objSource);
                GameObjectUtility.SetStaticEditorFlags(prefab, StaticEditorFlags.BatchingStatic | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic);
                Debug.Log("Overwritten prefab at " + destinyPath);
            }
        }
        else
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            GameObject objSource = (GameObject)PrefabUtility.InstantiatePrefab(go);
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(objSource, destinyPath);
            DestroyImmediate(objSource);
            GameObjectUtility.SetStaticEditorFlags(prefab, StaticEditorFlags.BatchingStatic | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.ReflectionProbeStatic);
            Debug.Log("Created prefab at " + destinyPath);
        }
        AssetDatabase.Refresh();
    }
}

#endif
