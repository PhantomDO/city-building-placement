using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Assets.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public enum Zone : int
{
    NaN = -1,
    Road = 0,
    CentreVille = 1,
    Residentiel = 2,
    Parc = 3,
    Etang = 4,
    Industriel = 5,
}

[Serializable]
public enum Building : int
{
    NaN = -1,
    Parc = 0,
    Maison = 1,
    PetitImmeuble = 3,
    Immeuble = 6,
    GrosImmeuble = 9,
    Usine = 10,
    Etang = 11,
}

[Serializable]
public struct CityCase
{
    public Zone zone;
    public Building building;
    public bool occupied;

    public static Vector3 GetBuildingSize(CityCase cityCase)
    {
        Vector3 size = Vector3.zero;

        if (cityCase.zone != Zone.NaN)
        {
            switch (cityCase.building)
            {
                case Building.NaN:
                case Building.Parc:
                case Building.Etang:
                    size = new Vector3(1, 0.1f, 1);
                    break;
                case Building.Usine:
                case Building.Maison:
                    size = Vector3.one;
                    break;
                case Building.PetitImmeuble:
                    size = new Vector3(1f, 2f, 1f);
                    break;
                case Building.Immeuble:
                    size = new Vector3(1f, 4f, 1f);
                    break;
                case Building.GrosImmeuble:
                    size = new Vector3(1f, 6f, 1f);
                    break;
            }
        }

        return size;
    }
}

public class CityBuild : MonoBehaviour
{
    public List<CityClass> epicentres;

    public int neighborHoodSize = 3;
    public int neighborHoodCount = 20;

    public GenericDictionary<Building, Color> buildingColor;

    public CityCase[] MapCase { get; private set; }
    public int DimensionSize { get; private set; }

    private void Awake()
    {
        DimensionSize = (neighborHoodSize + 1) * neighborHoodCount;
        MapCase = new CityCase[DimensionSize * DimensionSize];
    }

    void Start()
    {
        for (int i = 0; i < DimensionSize; i++)
        {
            for (int j = 0; j < DimensionSize; j++)
            {
                int index = i * DimensionSize + j;
                MapCase[index] = new CityCase
                {
                    zone = Zone.NaN,
                    building = Building.NaN,
                    occupied = false
                };
            }
        }

        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate += CreateCity;
        }

        CreateCity(epicentres[0]);
    }

    void OnDestroy()
    {
        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate -= CreateCity;
        }
    }

    private void CreateCity(CityClass city)
    {
        foreach (var ville in epicentres)
        {
            CreateEpicentres(ville);
        }

        CreateRiver();
        CreateRoads(neighborHoodSize + 1);
        
        for (int i = 0; i < DimensionSize; i += (neighborHoodSize + 1))
        {
            for (int j = 0; j < DimensionSize; j += (neighborHoodSize + 1))
            {
                CreateBuildings(i, j);
            }
        }

        if (TryGetComponent(out CityRenderer renderer))
        {
            renderer.Setup();
        }
    }

    private void CreateEpicentres(CityClass city)
    {
        for (float i = 0; i < city.superficyRadius; i+=0.1f)
        {
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.005f)
            {
                int x = (int)(city.position.x + i * Mathf.Cos(theta));
                int y = (int)(city.position.y + i * Mathf.Sin(theta));

                int index = y * DimensionSize + x;
                if (x >= 0 && x < DimensionSize && y >= 0 && y < DimensionSize)
                {
                    if(MapCase[index].zone != Zone.CentreVille)
                        MapCase[index].zone = i < city.superficyRadius * city.partieCentreVille ? Zone.CentreVille : Zone.Residentiel;

                    if (Random.Range(0.0f, 100.0f) < LoiNormale(i, 0, city.etendue) * city.densite)
                    {
                        MapCase[index].occupied = true;
                    }

                    //creer une zone industrielle :
                    //- pas dans le centre ville, ni en dehors de la ville (= dans la zone résidentielle)
                    //- 0.05% de chance d'apparition d'une ZI sur l'un des bâtiments
                    if (MapCase[index].zone == Zone.Residentiel && Random.Range(0f, 1f) >= 0.999995f)
                    {
                        Debug.Log($"Create industrial zone");
                        CreateIndustrialZone(x, y);
                    }
                }
            }
        }
    }

    private void CreateIndustrialZone(int posX, int posY)
    {
        // TODO : faire la zone
        float radius = Random.Range(0.05f, 0.10f);

        for (float i = 0; i < radius; i+=0.01f)
        {
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.01f)
            {
                int x = (int)(posX + i * 100 * Mathf.Cos(theta));
                int y = (int)(posY + i * 100 * Mathf.Sin(theta));

                int index = y * DimensionSize + x;
                if (x >= 0 && x < DimensionSize && y >= 0 && y < DimensionSize)
                {
                    MapCase[index].zone = Zone.Industriel;
                }
            }
        }
    }

    private void CreateBuildings(int i, int j)
    {
        int count = 0;

        //compte du nombre de batiments dans un quartier
        for (int k = 0; k < neighborHoodSize + 1; ++k)
        {
            for (int l = 0; l < neighborHoodSize + 1; ++l)
            {
                int index = (i + k) * DimensionSize + (j + l);

                if (MapCase[index].occupied == true && 
                    MapCase[index].zone != Zone.Road && MapCase[index].zone != Zone.Etang)
                {
                    count++;
                }
            }
        }

        //type de batiment selon la zone et le nombre de batiment du quartier
        for (int k = 0; k < neighborHoodSize + 1; ++k)
        {
            for (int l = 0; l < neighborHoodSize + 1; ++l)
            {
                int index = (i + k) * DimensionSize + (j + l);
                
                //parc au niveau des routes autour des parcs
                //if (MapCase[index].zone == Zone.Road && count == 0)
                //{
                //    MapCase[index].building = Building.Parc;
                //    MapCase[index].zone = Zone.Parc;
                //}
                if (MapCase[index].zone == Zone.Road || 
                    MapCase[index].zone == Zone.Etang ||
                    MapCase[index].zone == Zone.NaN)
                {
                    continue;
                }

                if (MapCase[index].zone == Zone.Industriel && MapCase[index].occupied == true)
                {
                    MapCase[index].building = Building.Usine;
                    continue;
                }
                else if (count == neighborHoodSize * neighborHoodSize && MapCase[index].zone == Zone.CentreVille) //GrosImmeuble only in center
                {
                    MapCase[index].building = Building.GrosImmeuble;
                }
                else if (count >= neighborHoodSize * (neighborHoodSize - 1))
                {
                    MapCase[index].building = Building.Immeuble;
                }
                else if (count >= neighborHoodSize)
                {
                    MapCase[index].building = Building.PetitImmeuble;
                }
                else if (count >= 1)
                {
                    MapCase[index].building = Building.Maison;
                }
                else if (count == 0)
                {
                    MapCase[index].building = Building.Parc;
                    MapCase[index].zone = Zone.Parc;
                    MapCase[index].occupied = true;
                }
                
            }
        }
        
    }

    private void CreateRiver() //nbr de pts de la rivière
    {
        List<Vector3> ptsRiver = new List<Vector3>();

        ptsRiver.Add(new Vector3(0, Random.Range(0, DimensionSize - 1), 0));
		foreach (var epicenter in epicentres)
		{
            ptsRiver.Add(epicenter.position);
        }
        ptsRiver.Add(new Vector3(DimensionSize, Random.Range(0, DimensionSize - 1),0));

        foreach (var v in bezier(ptsRiver)) {
            int index = ((int)v.y * DimensionSize + (int)v.x);
            if (index >= MapCase.Length || index < 0) continue;
            MapCase[index].occupied = true;
            MapCase[index].zone = Zone.Etang;
            MapCase[index].building = Building.Etang;
        }
    }

    
    private void CreateRoads(int spaceBetweenRoad)
    {
        // create road
        for (int y = 0; y < (neighborHoodSize + 1) * neighborHoodCount; ++y) 
        {
            for (int x = 0; x < (neighborHoodSize + 1) * neighborHoodCount; ++x) 
            {
                int index = y * DimensionSize + x;
                if ((y % spaceBetweenRoad == 0 || x % spaceBetweenRoad == 0) && MapCase[index].zone != Zone.NaN)
                {
                    MapCase[index].zone = Zone.Road;
                    MapCase[index].occupied = true;
                }
            }
        }
    }
    

    /*  Création zone rural, non utilisé
    [Range(0, 100)] public int parcDensity = 15;
    [Range(0, 100)] public int etangDensity = 20;
    private void CreateRuralZone(CityClass city)
    {
        var center = new Vector2Int(MapCase.GetLength(0) / 2 + 1, MapCase.GetLength(1) / 2 + 1);
        var minRadiusOverCenter = Mathf.FloorToInt(city.superficyRadius * 0.2f);

        var maxEtangArea = new Vector2Int(Mathf.FloorToInt(etangDensity / 2f), Mathf.FloorToInt(etangDensity / 2f));
        var maxParcArea = new Vector2Int(Mathf.FloorToInt(parcDensity / 2f), Mathf.FloorToInt(parcDensity / 2f));

        // create parc and water zone
        int countY = 0, countX = 0;
        for (int y = 0; y < MapCase.GetUpperBound(0); ++y)
        {
            for (int x = 0; x < MapCase.GetUpperBound(1); ++x)
            {
                // all the cube inside the radius of the center aren't considered
                if (x < center.x - minRadiusOverCenter || x > center.x + minRadiusOverCenter)
                {
                    countX++;
                }
            }

            countY++;
        }
    }
    */

    private float LoiNormale(float x, float esperance, float etendue)
	{
        //esperance = centre de la courbe
        return (1 / (etendue * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-0.5f * Mathf.Pow(((x - esperance) / etendue), 2));
    }

    int factorielle(int n)
    {
        if (n <= 1)
            return 1;
        return n * factorielle(n - 1);
    }
    float berstein(float t, int i, int n)
    {
        return (factorielle(n) / (factorielle(i) * factorielle(n - i))) * Mathf.Pow(t, i) * Mathf.Pow(1 - t, n - i);
    }
    Vector3[] bezier(List<Vector3> obj)
    {
        List<Vector3> pts = new List<Vector3>();
        int n = obj.Count; //nbr de points

        for (float t = 0; t <= 1.1; t += 0.001f)
        {
            Vector3 p = Vector3.zero;
            for (int i = 0; i < n; i++)
            {
                p += berstein(t, i, n - 1) * obj[i];
            }
            pts.Add(p);
        }
        return pts.ToArray();
    }
}
