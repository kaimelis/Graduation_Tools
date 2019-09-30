#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Sirenix.OdinInspector.Editor;

public class MaterialProcess : OdinEditorWindow
{
    [MenuItem("TBS/Tools/Material Process", priority = 0)]
    public static void ShowWindow()
    {
        GetWindow<MaterialProcess>().Show();
    }

    [TabGroup("MAIN", "MATERIALS")]
    [AssetList(Path = "/Models/", AutoPopulate = true)]
    [InlineEditor(InlineEditorModes.SmallPreview)]
    public List<Material> Materials;

    [TabGroup("MAIN", "MATERIALS")]
    [Button("Materials Prepare - All Selected Materials")]
    public void ProcessMaterials()
    {
        foreach (Material m in Materials)
        {
            ProcessMaterial(m);
        }
    }

    private void ProcessMaterial(Material m)
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            string assetPath = AssetDatabase.GetAssetPath(m);
            string directory = Path.GetDirectoryName(assetPath) + @"\";

            string[] aFilePaths = Directory.GetFiles(directory);
            foreach (var item in aFilePaths)
            {
                if (Path.GetExtension(item) == ".png" || Path.GetExtension(item) == ".jpg")
                {
                    Texture2D objAsset = AssetDatabase.LoadAssetAtPath(item, typeof(Texture2D)) as Texture2D;
                    if (m.name + "D" == objAsset.name)
                    {
                        m.SetTexture("_MainTex", objAsset);
                        Debug.Log(objAsset.name);
                    }
                    if (m.name + "AO" == objAsset.name)
                    {
                        m.SetTexture("_OcclusionMap", objAsset);
                        Debug.Log(objAsset.name);
                    }
                    if (m.name + "M" == objAsset.name)
                    {
                        m.SetTexture("_MetallicGlossMap", objAsset);
                        Debug.Log(objAsset.name);
                    }
                    if (m.name + "N" == objAsset.name)
                    {
                        m.SetTexture("_BumpMap", objAsset);
                        Debug.Log(objAsset.name);
                    }
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }

    [TabGroup("MAIN", "PREFABS")]
    [AssetList(Path = "/Models/", AutoPopulate = true)]
    [InlineEditor(InlineEditorModes.SmallPreview)]
    public List<GameObject> Prefabs = new List<GameObject>();

    private bool IsPrefab(Object obj)
    {
        return PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.Regular;
    }

    [TabGroup("MAIN", "PREFABS")]
    [Button("Assign materials to all selected models")]
    public void AssignMaterials()
    {
        foreach (GameObject m in Prefabs)
        {
            ProcessPrefabs(m);
        }
    }

    private void ProcessPrefabs(GameObject prefab)
    {
        string prefabName = prefab.name;
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        string folderPath = Path.GetDirectoryName(assetPath) + @"\";
        string[] aFilePaths = Directory.GetFiles(folderPath);
        Material mat = null;
        foreach (var item in aFilePaths)
        {
            if (Path.GetExtension(item) == ".mat")
            {
                Material objAsset = AssetDatabase.LoadAssetAtPath(item, typeof(Material)) as Material;
                mat = objAsset;
            }
        }

        var meshRenderer = prefab.GetComponentsInChildren<Renderer>();
        foreach (var item in meshRenderer)
        {
            if (mat != null)
                item.sharedMaterial = mat;
            Debug.Log(item.sharedMaterial.name);
        }
    }

}

#endif
