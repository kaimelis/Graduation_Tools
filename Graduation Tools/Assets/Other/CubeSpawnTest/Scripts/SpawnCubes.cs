using UnityEngine;
using Sirenix.OdinInspector;

namespace Custom.Tool.Grid
{
    public class SpawnCubes : MonoBehaviour
    {
        public int WorldWidth = 50;
        public int WorldHeight = 50;
        public GameObject PrefabCube;
        public Material RedMaterial, BlueMaterial, YellowMaterial;
        public bool UseRandomColor = true;

        [Button]
        public void CleanChildren()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                DestroyImmediate(gameObject.transform.GetChild(i).gameObject);
            }
        }

        [ButtonGroup]
        public void FillAllGrid()
        {
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int z = 0; z < WorldHeight; z++)
                {
                    GameObject block = Instantiate(PrefabCube, Vector3.zero, PrefabCube.transform.rotation) as GameObject;
                    block.transform.parent = transform;
                    block.transform.localPosition = new Vector3(x, 0, z);

                    if (UseRandomColor)
                    {
                        int randomRange = Random.Range(0, 3);
                        bool boolBlue = (randomRange == 0);
                        bool boolRed = (randomRange == 1);
                        bool boolYellow = (randomRange == 2);
                        if (boolBlue)
                        {
                            block.GetComponent<Renderer>().material = BlueMaterial;

                        }
                        else if (boolRed)
                        {
                            block.GetComponent<Renderer>().material = RedMaterial;
                        }
                        else if (boolYellow)
                        {
                            block.GetComponent<Renderer>().material = YellowMaterial;
                        }
                    }
                }
            }
        }

        [ButtonGroup]
        public void FillRandomlyGrid()
        {
            for (int x = 0; x < WorldWidth; x++)
            {
                for (int z = 0; z < WorldHeight; z++)
                {
                    bool Boolean = (Random.Range(0, 2) == 0);
                    if (Boolean)
                    {
                        GameObject block = Instantiate(PrefabCube, Vector3.zero, PrefabCube.transform.rotation) as GameObject;
                        block.transform.parent = transform;
                        block.transform.localPosition = new Vector3(x, 0, z);
                        if (UseRandomColor)
                        {
                            int randomRange = Random.Range(0, 3);
                            bool boolBlue = (randomRange == 0);
                            bool boolRed = (randomRange == 1);
                            bool boolYellow = (randomRange == 2);
                            if (boolBlue)
                            {
                                block.GetComponent<Renderer>().material = BlueMaterial;

                            }
                            else if (boolRed)
                            {
                                block.GetComponent<Renderer>().material = RedMaterial;
                            }
                            else if (boolYellow)
                            {
                                block.GetComponent<Renderer>().material = YellowMaterial;
                            }
                        }
                    }
                }
            }
        }

    }
}
