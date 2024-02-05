using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public GameObject MapWall;
    public int MapSize;
    public int tileSize;
    public float density; // Add a density field to control noise

    private int mapTileAmount;

    // Start is called before the first frame update
    void Start()
    {
        int[,] Map = init2dMap(MapSize, tileSize, density); // Pass density here

        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                if (Map[i, j] == 1) // Assuming 1 represents wall
                {
                    Instantiate(MapWall, new Vector3(i * tileSize, 0, j * tileSize), Quaternion.identity);
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
        Map = generateNoise(Map, density);
        return Map;
    }

    int[,] generateNoise(int[,] map, float density)
    {
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f); // Generate a random value
                map[i, j] = randomValue > density ? 0 : 1; // Assign wall or floor based on density
            }
        }
        return map;
    }

    
}
