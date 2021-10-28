using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuild : MonoBehaviour
{

    public List<CityClass> epicentres;

    private CityZone[,] mapZone = new CityZone[1029, 1029]; //must be a multiple of 3
    private typeBuilding[,] mapBuilding = new typeBuilding[1029, 1029]; //must be a multiple of 3

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= mapZone.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mapZone.GetUpperBound(1); j++)
            {
                mapZone[i, j] = CityZone.NaN;
                mapBuilding[i, j] = typeBuilding.NaN;
            }
        }
        createCity();
    }
	private void OnValidate()
	{
		for (int i = 0; i <= mapZone.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= mapZone.GetUpperBound(1); j++)
			{
                mapZone[i, j] = CityZone.NaN;
                mapBuilding[i, j] = typeBuilding.NaN;
            }
		}
        createCity();
    }
	void OnDrawGizmos()
    {
        /*
        //zone
        for (int i = 0; i <= mapZone.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mapZone.GetUpperBound(1); j++)
            {
				if (mapZone[i,j] == CityZone.CentreVille)
				{
                    Gizmos.color = new Color(1, 0, 0, 1f);
                    Gizmos.DrawCube(new Vector3(i,0,j), new Vector3(1, 1, 1));
                }
                else if (mapZone[i, j] == CityZone.Residentiel)
                {
                    Gizmos.color = new Color(0, 1, 0, 1f);
                    Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1, 1, 1));
                }
            }
		}
        */
        //building
        for (int i = 0; i <= mapBuilding.GetUpperBound(0); i++)
        {
            for (int j = 0; j <= mapBuilding.GetUpperBound(1); j++)
            {
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

    public enum CityZone : int
    {
        NaN = -1,
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
        GrosImmeuble = 9 
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
                    if (x >= 0 && x < 1000 && y >= 0 && y < 1000)
                    {
                        if (Random.Range(0, 100) < loiNormale(i, 0, ville.etendue) * ville.densite)
                        {
                            if (i < ville.superficyRadius * ville.partieCentreVille)
                                mapZone[(int)x, (int)y] = CityZone.CentreVille;
                            else if (mapZone[(int)x, (int)y] == CityZone.CentreVille) //d�j� un centre ville d'un autre �picentre
                                continue;
                            else
                                mapZone[(int)x, (int)y] = CityZone.Residentiel;
                        }
                    }
                }
            }
        }
        //cr�ation building
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


    private float loiNormale(float x, float esperance, float etendue)
    {
        //esperance = centre de la courbe
        return (1 / (etendue * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-0.5f * Mathf.Pow(((x - esperance) / etendue), 2));
    }
}
