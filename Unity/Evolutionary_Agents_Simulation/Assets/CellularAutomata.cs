using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{

    public GameObject MapWall;
    public int MapSize;
    public int tileSize;
    public float densityNoise;
    
    private Vector3 position;
    private int mapTileAmount;
   


    // Start is called before the first frame update
    void Start()
    {

        int[,] Map = init2dMap(MapSize, tileSize, densityNoise);

        Debug.Log(mapTileAmount);


        for (int i = 0; i < mapTileAmount; i++)
        {

            for (int j = 0; j < mapTileAmount; j++)
            {
                if (Map[i,j] == 1)
                {
                    Instantiate(MapWall, new Vector3(i, 0, j), transform.rotation);

                }


            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int[,] init2dMap(int MapSize, int tileSize, float density)
    {
        mapTileAmount = MapSize / tileSize;
        int[,] Map = new int[mapTileAmount, mapTileAmount];

        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                float random = UnityEngine.Random.Range(0f, 100f); // Using UnityEngine's Random
                Map[i, j] = random > density ? 0 : 1; // Assuming 0 is floor and 1 is wall
            }
        }

        return Map;
    }


}
