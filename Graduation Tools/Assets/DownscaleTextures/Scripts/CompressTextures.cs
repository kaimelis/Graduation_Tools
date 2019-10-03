#if UNITY_EDITOR
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public class CompressTextures : OdinEditorWindow
{
    [MenuItem("CustomTools/Texture Compress", priority = 0)]
    public static void ShowWindow()
    {
        GetWindow<CompressTextures>().Show();
    }

    [TabGroup("MAIN", "TEXTURE")]
    public int TextureSize = 512;

    [TabGroup("MAIN", "TEXTURE")]
    [AssetList(CustomFilterMethod = "IsTexture", Path = "Models/", AutoPopulate = true)]
    [InlineEditor(InlineEditorModes.SmallPreview)]
    public List<Texture> Textures;

    [TabGroup("MAIN", "TEXTURE")]
    [Button("Compress Textures")]
    public void ProcessTextures()
    {
        foreach (Texture m in Textures)
        {
            ProcessTexture(m);
        }
    }

    private void ProcessTexture(Texture m)
    {
        try
        {
            AssetDatabase.StartAssetEditing();
            string assetPath = AssetDatabase.GetAssetPath(m);

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
            TextureImporterSettings settings = new TextureImporterSettings();
            importer.ReadTextureSettings(settings);
            TextureImporterPlatformSettings platformSettings = importer.GetDefaultPlatformTextureSettings();
            platformSettings.maxTextureSize = TextureSize;
            importer.SetPlatformTextureSettings(platformSettings);
            importer.SaveAndReimport();

        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
        }
    }

    private bool IsTexture(Texture obj)
    {
        if (obj.width >= 1028 && obj.height >= 1028)
            return true;

        else
            return false;
    }

}
#endif