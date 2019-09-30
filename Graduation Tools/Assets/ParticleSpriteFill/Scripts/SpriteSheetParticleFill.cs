#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
public class SpriteSheetParticleFill : MonoBehaviour
{
    public Texture2D AnimatedSprite;
    private ParticleSystem particle;

    [ButtonGroup]
    private void CleanSpriteList()
    {
        particle = GetComponent<ParticleSystem>();
        for (int i = 0; i <= particle.textureSheetAnimation.spriteCount; i++)
        {
            particle.textureSheetAnimation.RemoveSprite(i);
            if (particle.textureSheetAnimation.spriteCount == 0)
                break;
            else
                CleanSpriteList();
        }
    }

    [ButtonGroup]
    private void FillSpriteList()
    {
        particle = GetComponent<ParticleSystem>();
        for (int i = 0; i < particle.textureSheetAnimation.spriteCount; i++)
        {
            particle.textureSheetAnimation.RemoveSprite(i);
        }

        string path = AssetDatabase.GetAssetPath(AnimatedSprite);
        string directory = Path.GetDirectoryName(path) + @"\";
        string[] aFilePaths = Directory.GetFiles(directory);

        foreach (var item in aFilePaths)
        {
            if (Path.GetExtension(item) == ".png" || Path.GetExtension(item) == ".jpg")
            {
                Sprite objAsset = AssetDatabase.LoadAssetAtPath(item, typeof(Sprite)) as Sprite;
                Object[] objects = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(AnimatedSprite));
                Sprite[] sprites = objects.Select(x => (x as Sprite)).ToArray();
                for (int i = 0; i < sprites.Length; i++)
                {
                    particle.textureSheetAnimation.AddSprite(sprites[i]);
                }
            }
        }
    }
}

#endif
