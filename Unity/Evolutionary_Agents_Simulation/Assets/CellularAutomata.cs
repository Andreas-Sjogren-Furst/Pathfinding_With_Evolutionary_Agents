using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{
    public GameObject MapWall;
    public GameObject Plane;
    private int MapSize;
    public int tileSize;
    public float density; // Add a density field to control noise
    public int CellularIterations;

    private int mapTileAmount;

    // Start is called before the first frame update
    void Start()
    {
        MapSize = (int)(Plane.transform.localScale.x * Plane.transform.localScale.x);
        MapWall.transform.localScale = new Vector3(tileSize,MapWall.transform.localScale.y,tileSize);
        Debug.Log(MapSize);

        int[,] Map = init2dMap(MapSize, tileSize, density, CellularIterations); // Pass density here
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                if (Map[i, j] == 1) // Assuming 1 represents wall
                {   
                    
                    Instantiate(MapWall, transform.position + new Vector3(i * tileSize, 0, j * tileSize) + new Vector3((float)(tileSize / 2.0), 0 , (float)(tileSize / 2.0)), Quaternion.identity);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    int[,] init2dMap(int MapSize, int tileSize, float density, int iterations)
    {
        mapTileAmount = MapSize / tileSize;
        int[,] Map = new int[mapTileAmount, mapTileAmount];
        Map = generateNoise(Map, density);
        Map = applyCellularAutomaton(Map, iterations);

        return Map;
    }

    int[,] generateNoise(int[,] map, float density)
    {
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f); // Generate a random value
                map[i, j] = randomValue > density ? 0 : 1; // Assign floor or wall based on density.
            }
        }
        return map;
    }


    int[,] applyCellularAutomaton(int[,] grid, int count)
    {
        int width = grid.GetLength(1);
        int height = grid.GetLength(0);

        for (int i = 0; i < count; i++)
        {
            int[,] tempGrid = (int[,])grid.Clone();

            for (int j = 0; j < height; j++)
            {
                for (int k = 0; k < width; k++)
                {
                    int neighborWallCount = 0;

                    for (int y = j - 1; y <= j + 1; y++)
                    {
                        for (int x = k - 1; x <= k + 1; x++)
                        {
                            if (x >= 0 && x < width && y >= 0 && y < height)
                            {
                                if (y != j || x != k)
                                {
                                    if (tempGrid[y,x] == 1)
                                    {
                                        neighborWallCount++;
                                    }
                                }
                              //  if (!(y == j && x == k) && tempGrid[y, x] == 1) // Assuming 1 represents wall
                            //    {
                             //       neighborWallCount++;
                            //    }
                            }
                            else
                            {
                                neighborWallCount++; // Increment if out of bounds, assuming border as wall
                            }
                        }
                    }

                    grid[j, k] = neighborWallCount > 4 ? 1 : 0; // Update based on neighbor count
                }
            }
        }
        return grid;
    }



}
