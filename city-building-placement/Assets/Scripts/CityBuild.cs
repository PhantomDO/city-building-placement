using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum Zone : int
{
    NaN = -1,
    Road = 0,
    CentreVille = 1,
    Residentiel = 2,
    Parc = 3,
    Etang = 4,
}

public enum Building : int
{
    NaN = -1,
    Parc = 0,
    Maison = 1,
    PetitImmeuble = 3,
    Immeuble = 6,
    GrosImmeuble = 9
}

[Serializable]
public struct CityCase
{
    public Zone zone;
    public Building building;
}

public class CityBuild : MonoBehaviour
{
    public List<CityClass> epicentres;

    [Range(0, 100)] public int parcDensity = 15;
    [Range(0, 100)] public int etangDensity = 20;

    public int neighborHoodSize = 3;
    public int neighborHoodCount = 20;

    private CityCase[] _mapCase;
    private int _dimensionSize;

    // Start is called before the first frame update
    void Start()
    {
        _dimensionSize = (neighborHoodSize + 1) * neighborHoodCount;
        _mapCase = new CityCase[_dimensionSize * _dimensionSize];

        for (int i = 0; i < _dimensionSize; i++)
        {
            for (int j = 0; j < _dimensionSize; j++)
            {
                int index = i * _dimensionSize + j;
                _mapCase[index] = new CityCase
                {
                    zone = Zone.NaN,
                    building = Building.NaN
                };
            }
        }

        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate += createCity;
        }

        createCity(epicentres[0]);
    }

    void OnDestroy()
    {
        foreach (var city in epicentres)
        {
            city.OnAttributeUpdate -= createCity;
        }
    }

	void OnDrawGizmos()
    {
        if (_dimensionSize <= 0) return;

        for (int i = 0; i < _dimensionSize; i++)
        {
            for (int j = 0; j < _dimensionSize; j++)
            {
                int index = i * _dimensionSize + j;

                if (_mapCase[index].zone == Zone.Road)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1, 1, 1));
                    continue;
                }

                if (_mapCase[index].zone != Zone.NaN)
                {
                    
                    if (_mapCase[index].building == Building.Parc)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 1, 1f));
                    }
                    if (_mapCase[index].building == Building.Maison)
                    {
                        Gizmos.color = new Color(0, 1, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1, 1, 1));
                    }
                    else if (_mapCase[index].building == Building.PetitImmeuble)
                    {
                        Gizmos.color = new Color(1, 0, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 2, 1f));
                    }
                    else if (_mapCase[index].building == Building.Immeuble)
                    {
                        Gizmos.color = new Color(0, 0, 1, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 4, 1f));
                    }
                    else if (_mapCase[index].building == Building.GrosImmeuble)
                    {
                        Gizmos.color = new Color(1, 1, 0, 1f);
                        Gizmos.DrawCube(new Vector3(i, 0, j), new Vector3(1f, 6, 1f));
                    }
                }
            }
        }
    }

    private void createCity(CityClass city)
    {
        createRoads(neighborHoodSize + 1);
        
        foreach (var ville in epicentres)
        {
            createEpicentres(ville);
        }

        for (int i = 0; i < _dimensionSize; i += (neighborHoodSize + 1))
        {
            for (int j = 0; j < _dimensionSize; j += (neighborHoodSize + 1))
            {
                createBuildings(i, j);
            }
        }
    }

    private void createEpicentres(CityClass city)
    {
        for (int i = 0; i < city.superficyRadius; ++i)
        {
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.01f)
            {
                int x = (int)(city.position.x + i * Mathf.Cos(theta));
                int y = (int)(city.position.y + i * Mathf.Sin(theta));

                float curve = loiNormale(i, 0, city.etendue) * city.densite;

                int index = y * _dimensionSize + x;

                if ((x >= 0 && x < _dimensionSize && y >= 0 && y < _dimensionSize) &&
                    (_mapCase[index].zone != Zone.Road && _mapCase[index].zone != Zone.CentreVille) &&
                    Random.Range(0.0f, 100.0f) < curve)
                {
                    _mapCase[index].zone = i < city.superficyRadius * city.partieCentreVille ? Zone.CentreVille : Zone.Residentiel;
                }
            }
        }
    }

    private void createBuildings(int i, int j)
    {
        int count = 0;

        for (int k = 0; k < neighborHoodSize + 1; ++k)
        {
            for (int l = 0; l < neighborHoodSize + 1; ++l)
            {
                int index = (i + k) * _dimensionSize + (j + l);
                
                if (_mapCase[index].zone == Zone.NaN || _mapCase[index].zone == Zone.Road)
                {
                    continue;
                }
                
                count++;

                if (count == 0) // it's a parc
                {
                    _mapCase[index].building = Building.Parc;
                    Debug.Log("parc here");
                    Debug.Log((i + k) + ", " + (j + l));
                    Color color = new Color(1, 0, 1.0f);
                    Debug.DrawLine(new Vector3(i + k, 0, j + l), new Vector3(i + k + 1f, 0, j + l + 1f), color);
                }
                else
                {
                    if (count == neighborHoodSize * neighborHoodSize)
                    {
                        _mapCase[index].building = Building.GrosImmeuble;
                        return;
                    }

                    if (count >= neighborHoodSize * (neighborHoodSize - 1))
                    {
                        _mapCase[index].building = Building.Immeuble;
                        continue;
                    }

                    _mapCase[index].building = count >= neighborHoodSize ? Building.PetitImmeuble : Building.Maison;
                }
            }
        }
    }

    private void createRoads(int spaceBetweenRoad)
    {
        // create road
        for (int y = 0; y < (neighborHoodSize + 1) * neighborHoodCount; ++y) 
        {
            for (int x = 0; x < (neighborHoodSize + 1) * neighborHoodCount; ++x) 
            {
                int index = y * _dimensionSize + x;
                if (y % spaceBetweenRoad == 0 || x % spaceBetweenRoad == 0)
                {
                    _mapCase[index].zone = Zone.Road;
                }
            }
        }
    }

    private void createRuralZone(CityClass city)
    {
        var center = new Vector2Int(_mapCase.GetLength(0) / 2 + 1, _mapCase.GetLength(1) / 2 + 1);
        var minRadiusOverCenter = Mathf.FloorToInt(city.superficyRadius * 0.2f);

        var maxEtangArea = new Vector2Int(Mathf.FloorToInt(etangDensity / 2f), Mathf.FloorToInt(etangDensity / 2f));
        var maxParcArea = new Vector2Int(Mathf.FloorToInt(parcDensity / 2f), Mathf.FloorToInt(parcDensity / 2f));

        // create parc and water zone
        int countY = 0, countX = 0;
        for (int y = 0; y < _mapCase.GetUpperBound(0); ++y)
        {
            for (int x = 0; x < _mapCase.GetUpperBound(1); ++x)
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

    private float loiNormale(float x, float esperance, float etendue)
	{
        //esperance = centre de la courbe
        return (1 / (etendue * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-0.5f * Mathf.Pow(((x - esperance) / etendue), 2));
    }
}
