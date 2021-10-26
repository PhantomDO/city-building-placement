using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityBuild : MonoBehaviour
{

    public int superficyRadius; //city radius
    public int etendue; //city density
    public Vector2 position;

    private int[,] map = new int[1000, 1000];

    // Start is called before the first frame update
    void Start()
    {
        createCity();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 1f);
        for (int i = 0; i < 1000; i++)
		{
			for (int j = 0; j < 1000; j++)
			{
				if (map[i,j] == 1)
				{
                    Gizmos.DrawCube(new Vector3(i,0,j), new Vector3(1, 1, 1));
                }
			}
		}        
    }

    public void createCity()
    {
        float x, y;
		for (int i = 0; i < superficyRadius; i+=1)
		{
            for (float theta = 0; theta < 2 * Mathf.PI; theta += 0.01f)
            {
                x = position.x + i * Mathf.Cos(theta);
                y = position.y + i * Mathf.Sin(theta);
                x = (int)x;
                y = (int)y;
                if (x >= 0 && x < 1000 && y >= 0 && y < 1000){
                    if (Random.Range(0, 100) < loiNormale(i, 0))
                    {
                        map[(int)x, (int)y] = 1;
                    }
                }
            }
        }
    }


    private float loiNormale(float x, float esperance)
    {
        //esperance = centre de la courbe
        return (1 / (etendue * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-0.5f * Mathf.Pow(((x - esperance) / etendue), 2));
    }
}
