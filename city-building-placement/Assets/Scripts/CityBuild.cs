using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuild : MonoBehaviour
{

    public List<CityClass> epicentres;

    [Range(0, 100)] public int parcDensity = 15;
    [Range(0, 100)] public int etangDensity = 20;

    [SerializeField] private List<Vector2Int> parcs;
    [SerializeField] private List<Vector2Int> etangs;
    
    private CityZone[,] mapZone = new CityZone[129, 129]; //must be a multiple of 3
    private typeBuilding[,] mapBuilding = new typeBuilding[129, 129]; //must be a multiple of 3
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate += initializeCity;
        }

        initializeCity(epicentres[0]);
    }

    void OnDestroy()
    {
        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate -= initializeCity;
        }
    }

    private void initializeCity(CityClass city)
    {
        for (int i = 0; i <= mapZone.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mapZone.GetUpperBound(1); j++)
            {
                mapZone[i, j] = CityZone.NaN;
                mapBuilding[i, j] = typeBuilding.NaN;
            }
        }
        createRoads(4);
        createCity();
    }

	void OnDrawGizmos()
    {
        for (int i = 0; i <= mapZone.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mapZone.GetUpperBound(1); j++)
            {
                if (mapZone[i, j] == CityZone.Road)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1, 1, 1));
                    continue;
                }

                if (mapZone[i, j] != CityZone.NaN)
                {

                    if (mapBuilding[i, j] == typeBuilding.Maison)
                    {
                        Gizmos.color = new Color(0, 1, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1, 1, 1));
                    }
                    else if (mapBuilding[i, j] == typeBuilding.PetitImmeuble)
                    {
                        Gizmos.color = new Color(1, 0, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 2, 1f));
                    }
                    else if (mapBuilding[i, j] == typeBuilding.Immeuble)
                    {
                        Gizmos.color = new Color(0, 0, 1, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 4, 1f));
                    }
                    else if (mapBuilding[i, j] == typeBuilding.GrosImmeuble)
                    {
                        Gizmos.color = new Color(1, 1, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 6, 1f));
                    }
                }
            }
        }
    }

    public enum RuralZone : int
    {
        Nan = -1,
        Etang = 0,
        Parc = 1
    }

    public enum CityZone : int
    {
        NaN = -1,
        Road = 0,
        CentreVille = 1,
        Residentiel = 2,
        Parc = 3,
    }
    public enum typeBuilding : int
    {
        NaN = 0,
        Maison = 1,
        PetitImmeuble = 3,
        Immeuble = 6,
        GrosImmeuble = 9 //centre ville uniquement
    }
    public void createCity()
    {
        float x, y;
		foreach (var ville in epicentres)
		{
            for (int i = 0; i < ville.superficyRadius; i += 1)
            {
                for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.01f)
                {
                    x = ville.position.x + i * Mathf.Cos(theta);
                    y = ville.position.y + i * Mathf.Sin(theta);
                    x = (int)x;
                    y = (int)y;
                    if (x >= 0 && x < mapZone.GetLength(0) && y >= 0 && y < mapZone.GetLength(1))
                    {
                        if (Random.Range(0, 100) < loiNormale(i, 0, ville.etendue) * ville.densite)
                        {
                            if (i < ville.superficyRadius * ville.partieCentreVille)
                                mapZone[(int)x, (int)y] = CityZone.CentreVille;
                            else if (mapZone[(int)x, (int)y] == CityZone.CentreVille) //déjà un centre ville d'un autre épicentre
                                continue;
                            else
                                mapZone[(int)x, (int)y] = CityZone.Residentiel;
                        }
                    }
                }
            }
        }
        
        //création building
        for (int i = 0; i <= mapZone.GetUpperBound(0); i += 3)
        {
            for (int j = 0; j <= mapZone.GetUpperBound(1); j += 3)
            {
                int count = 0;
				for (int k = 0; k < 3; k++)
				{
					for (int l = 0; l < 3; l++)
					{
                        if (mapZone[i+k, j+l] != CityZone.NaN)
                            count++;
					}
				}
                if (count == 9)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            mapBuilding[i + k, j + l] = typeBuilding.GrosImmeuble;
                        }
                    }
                }
                else if (count >= 6)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            mapBuilding[i + k, j + l] = typeBuilding.Immeuble;
                        }
                    }
                }
                else if (count >= 3)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            mapBuilding[i + k, j + l] = typeBuilding.PetitImmeuble;
                        }
                    }
                }
                else if (count >= 1)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            mapBuilding[i + k, j + l] = typeBuilding.Maison;
                        }
                    }
                }
            }
        }
    }

    private void createRoads(int spaceBetweenRoad)
    {
        // create road
        for (int y = 0; y < mapZone.GetUpperBound(0); y++) // y
        {
            for (int x = 0; x < mapZone.GetUpperBound(1); x++) // x
            {
                if (y % spaceBetweenRoad == 0 || x % spaceBetweenRoad == 0)
                {
                    mapZone[x, y] = CityZone.Road;
                }
            }
        }
    }

    private void createRuralZone(CityClass city)
    {
        var center = new Vector2Int(mapZone.GetLength(0) / 2 + 1, mapZone.GetLength(1) / 2 + 1);
        var minRadiusOverCenter = Mathf.FloorToInt(city.superficyRadius * 0.2f);

        var maxEtangArea = new Vector2Int(Mathf.FloorToInt(etangDensity / 2f), Mathf.FloorToInt(etangDensity / 2f));
        var maxParcArea = new Vector2Int(Mathf.FloorToInt(parcDensity / 2f), Mathf.FloorToInt(parcDensity / 2f));

        // create parc and water zone
        int countY = 0, countX = 0;
        for (int i = 0; i < mapZone.GetUpperBound(0); i++) // y
        {
            for (int j = 0; j < mapZone.GetUpperBound(1); j++) // x
            {
                // all the cube inside the radius of the center aren't considered
                if (j < center.x - minRadiusOverCenter || j > center.x + minRadiusOverCenter)
                {
                    countX++;
                }
            }

            countY++;
        }
    }

    private float loiNormale(float x, float esperance, float etendue)
	{
        //esperance = centre de la courbe
        return (1 / (etendue * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-0.5f * Mathf.Pow(((x - esperance) / etendue), 2));
    }
}
