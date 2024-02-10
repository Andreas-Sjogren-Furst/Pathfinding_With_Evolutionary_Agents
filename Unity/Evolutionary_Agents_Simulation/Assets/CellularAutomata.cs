using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using System;
using System.Linq;

public class CellularAutomata : MonoBehaviour
{
    public GameObject MapWall;
    public GameObject Plane;
    public GameObject CheckPoint;
    private int MapSize;
    public int NumberOfCheckPoints;
    public int tileSize;
    public float density; // Add a density field to control noise
    public int CellularIterations;
    public int CheckPointSpacing;
    public int erosionLimit = 3;

    private int mapTileAmount;

    // Start is called before the first frame update
    void Start()
    {
        MapSize = (int)(Plane.transform.localScale.x * Plane.transform.localScale.x);
        MapWall.transform.localScale = new Vector3(tileSize,MapWall.transform.localScale.y,tileSize);
        
        int[,] Map = init2dMap(MapSize, tileSize, density, CellularIterations); // Pass density here
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                if (Map[i, j] == 1) //  1 represents wall
                {   
                    
                    Instantiate(MapWall, transform.position + new Vector3(i * tileSize, 0, j * tileSize) + new Vector3((float)(tileSize / 2.0), 0 , (float)(tileSize / 2.0)), Quaternion.identity);
                }
                else if (Map[i,j] == 2) // 2 represents checkpoint
                {
                    Instantiate(CheckPoint, transform.position + new Vector3(i * tileSize, 0, j * tileSize) + new Vector3((float)(tileSize / 2.0), 0, (float)(tileSize / 2.0)), Quaternion.identity);

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
        Map = generateCheckpoints(Map, NumberOfCheckPoints);
        Map = generateNoise(Map, density);
        Map = applyCellularAutomaton(Map, iterations, erosionLimit);


        return Map;
    }


    Boolean FloorOrWall(int cell)
    {

        if (cell > 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    int[,] generateNoise(int[,] map, float density)
    {
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                if (FloorOrWall(map[i, j])) {
                    float randomValue = UnityEngine.Random.Range(0f, 100f); // Generate a random value
                    map[i, j] = randomValue > density ? 0 : 1; // Assign floor or wall based on density.
                }
            }
        }

        // Prepare to track cells that need their neighbors set to 0 without overwriting the original cells
        List<(int, int)> cellsToExpand = new List<(int, int)>();

        // Identify cells that are not floor or wall
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                if (!FloorOrWall(map[i, j])) // If the cell is not a floor or wall
                {
                    cellsToExpand.Add((i, j));
                }
            }
        }

        // Set the neighbouring cells of identified cells to 0, avoiding overwriting the original cells, in a circular pattern
        foreach (var (i, j) in cellsToExpand)
        {
            for (int di = -CheckPointSpacing; di <= CheckPointSpacing; di++)
            {
                for (int dj = -CheckPointSpacing; dj <= CheckPointSpacing; dj++)
                {
                    int ni = i + di;
                    int nj = j + dj;
                    // Check bounds
                    if (ni >= 0 && ni < mapTileAmount && nj >= 0 && nj < mapTileAmount)
                    {
                        // Calculate the distance from the center point using the Euclidean distance formula
                        double distance = Math.Sqrt(di * di + dj * dj);

                        // Only clear cells within a circular area, defined by CheckPointSpacing as the radius
                        if (distance <= CheckPointSpacing)
                        {
                            if (FloorOrWall(map[ni, nj]))
                            {
                                map[ni, nj] = 0; // Set to 0
                            }
                        }
                    }
                }
            }
        }



        return map;
    }



    int[,] generateCheckpoints(int[,] grid, int numberOfCheckPoints)
    {
     //   int totalCells = (grid.GetLength(1) - 1) * (grid.GetLength(0) - 1);
        // Random checkpoint coordinates.

     //   int[] CheckPointCoordinates = new int[numberOfCheckPoints];

        for (int i = 0; i < numberOfCheckPoints; i++)
        {
            int randomXValue = UnityEngine.Random.Range(0, grid.GetLength(0));
            int randomYValue = UnityEngine.Random.Range(0, grid.GetLength(1));


            grid[randomXValue, randomYValue] = 2;




        }



     

        return grid;

    }


    int[,] applyCellularAutomaton(int[,] grid, int count, int erosionLimit)
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
                    if (FloorOrWall(grid[j, k])) {
                        grid[j, k] = neighborWallCount > erosionLimit ? 1 : 0; // Update based on neighbor count
                    }
                }
            }
        }
        return grid;
    }



}
