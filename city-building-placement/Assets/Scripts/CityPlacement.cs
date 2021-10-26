using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[System.Serializable]
public struct Bloc
{
    public int[] Pixels;
}

public class CityPlacement : MonoBehaviour
{
    public Texture2D RoadMap;
    public List<Bloc> Blocs = new List<Bloc>();
    public List<GameObject> Cubes = new List<GameObject>();
    //public RenderTexture OutTexture;

    public float fireRate = 0.02f;
    
    private void Start()
    {
        StartCoroutine(CreateBlocs());
    }

    public IEnumerator CreateBlocs()
    {
        var width = RoadMap.width;
        var height = RoadMap.height;
        var pixels = RoadMap.GetPixels();
        //OutTexture = new RenderTexture(width, height, height);

        Debug.Log($"Texture w: {width}, h: {height}, p: {pixels.Length}");
        
        Color oldPix = Color.black;
        List<int> pixelList = new List<int>();
        for (int y = 0; y < height; y++)
        {
            pixelList.Clear();
            oldPix = Color.black;
            for (int x = 0; x < width; x++)
            {
                //yield return new WaitForSeconds(fireRate);
                var pix = pixels[y * width + x];
				var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
				go.transform.position = Vector3.zero + new Vector3(x, 0, y);
				go.transform.parent = transform;

				if (!go.TryGetComponent(out MeshRenderer meshRenderer))
				{
					Debug.Log($"No mesh renderer found");
				}

				//Cubes.Add(go);

				if (pix == Color.white)
                {
                    pixelList.Add(y * width + x);
                    meshRenderer.material.color = Color.white;
                }
                else
                {
                    meshRenderer.material.color = Color.black;
                    if (oldPix == Color.white)
                    {
                        Blocs.Add(new Bloc { Pixels = pixelList.ToArray() });
                        pixelList.Clear();
                    }
                }
                
                oldPix = pix;
            }
        }

        //Camera.main.targetTexture = OutTexture;
        yield break;
    }
}
