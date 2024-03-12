using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MapController
{
    public static void ClearMap(ref List<GameObject> spawnedObjects)
    {
        foreach (GameObject obj in spawnedObjects)
        {
            CellularAutomata.ViewDestroyObject(obj);
        }
        // Clear the list
        spawnedObjects.Clear();
    }

    public static void CreateMap(ref List<GameObject> spawnedObjects, GameObject MapWall,
                     GameObject CheckPoint, GameObject GridTile, Vector3 currentTransformPosition, List<Vector3> antSpawnerWorldPositions, MapModel mapModel)
    {
        // mapModel.mapTileAmount = mapModel.mapSize / mapModel.TileSize;
        int[,] Map = init2dMap(mapModel.mapSize, mapModel.TileSize, mapModel.Density, mapModel.CellularIterations, mapModel.NumberOfCheckPoints, antSpawnerWorldPositions, mapModel.ErosionLimit, mapModel.CheckPointSpacing, currentTransformPosition, mapModel.mapTileAmount);
        for (int i = 0; i < mapModel.mapTileAmount; i++)
        {
            for (int j = 0; j < mapModel.mapTileAmount; j++)
            {
                Vector3 position = currentTransformPosition + new Vector3(i * mapModel.TileSize, 0, j * mapModel.TileSize) + new Vector3((float)(mapModel.TileSize / 2.0), 0, (float)(mapModel.TileSize / 2.0));
                if (Map[i, j] == 0) // 1 represents wall
                {
                    GameObject wall = CellularAutomata.ViewInistiateObject(MapWall, position);
                    spawnedObjects.Add(wall);
                }
                else if (Map[i, j] == 1) // 1 represents wall
                {
                    GameObject wall = CellularAutomata.ViewInistiateObject(MapWall, position);
                    spawnedObjects.Add(wall);
                }
                else if (Map[i, j] == 2) // 2 represents checkpoint
                {
                    GameObject checkpoint = CellularAutomata.ViewInistiateObject(CheckPoint, position);
                    spawnedObjects.Add(checkpoint);
                }
                else if (Map[i, j] == 3)
                {
                    // Spawned already made. 

                }
            }
        }
    }




    static int[,] init2dMap(int MapSize, int tileSize, float density, int iterations, int numberOfCheckPoints,
                    List<Vector3> antSpawnerWorldPositions, int erosionLimit, int checkPointSpacing, Vector3 currentTransformPosition, int mapTileAmount)
    {
        mapTileAmount = MapSize / tileSize;
        int[,] Map = new int[mapTileAmount, mapTileAmount];
        Map = generateCheckpoints(Map, numberOfCheckPoints);
        //      int[,] checkpointCoordinates = getObjectCoordinates(Map, NumberOfCheckPoints, 2);

        Map = RemoveWallsFromRealWorldPosition(Map, antSpawnerWorldPositions, tileSize, 3, currentTransformPosition);
        Map = generateNoise(Map, density, checkPointSpacing, mapTileAmount);
        Map = applyCellularAutomaton(Map, iterations, erosionLimit);
        checkIfpathExists(Map, numberOfCheckPoints);

        return Map;
    }


    static Boolean FloorOrWall(int cell)
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

    static int[,] generateNoise(int[,] map, float density, int CheckPointSpacing, int mapTileAmount)
    {

        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {

                if (FloorOrWall(map[i, j]))
                {
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



    static int[,] generateCheckpoints(int[,] grid, int numberOfCheckPoints)
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

    static int[,] getObjectCoordinates(int[,] grid, int numberOfCheckPoints, int ObjectValue)
    {
        int[,] CheckPointCoordinates = new int[numberOfCheckPoints, 2];
        int counter = 0;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (grid[i, j] == ObjectValue)
                {
                    CheckPointCoordinates[counter, 0] = i;
                    CheckPointCoordinates[counter, 1] = j;
                    counter++;
                }
                if (counter == numberOfCheckPoints)
                {
                    break;
                }
            }
        }
        return CheckPointCoordinates;
    }

    static Boolean checkIfpathExists(int[,] Map, int numberOfCheckPoints)
    {

        Astar aStar = new Astar();

        // Boolean pathExists = false;
        int counter = 0;

        int[,] checkpoints = getObjectCoordinates(Map, numberOfCheckPoints, 2);
        int[,] spawnerCoordinates = getObjectCoordinates(Map, 1, 3);
        List<Vector2Int>[] paths = new List<Vector2Int>[checkpoints.Length];

        Debug.Log($"map length");

        Debug.Log("Checkpoint coordinates: " + checkpoints.GetLength(0));
        Debug.Log("Spawner coordinates: " + spawnerCoordinates.GetLength(0));

        List<Vector2Int> combinedPath = new List<Vector2Int>();


        //  while (!pathExists)
        {
            counter++;
            if (counter > 3)
            {
                Debug.Log("Path does not exist");
                return false;
                // break;
            }



            if (spawnerCoordinates.GetLength(0) == 0 || spawnerCoordinates[0, 0] == 0 || spawnerCoordinates[0, 1] == 0)
            {
                Debug.Log("No spawner found");
                // break;
                return false;
            }
            if (checkpoints.GetLength(0) == 0)
            {
                Debug.Log("No checkpoints found");
                return false;
                // break;
            }

            for (int i = 0; i < checkpoints.GetLength(0); i++)
            {
                for (int j = 0; j < spawnerCoordinates.GetLength(0); j++)
                {
                    Debug.Log("Checkpoint: " + checkpoints[i, 0] + " " + checkpoints[i, 1]);
                    Debug.Log("Spawner: " + spawnerCoordinates[j, 0] + " " + spawnerCoordinates[j, 1]);
                    Debug.Log("running a star");

                    Vector2Int tempCheckPoint = new Vector2Int(checkpoints[i, 0], checkpoints[i, 1]);
                    Vector2Int spawner2d = new Vector2Int(spawnerCoordinates[j, 0], spawnerCoordinates[j, 1]);

                    Debug.Log("Checkpoint: " + tempCheckPoint);
                    Debug.Log("Spawner: " + spawner2d);
                    paths[i] = aStar.FindPath(tempCheckPoint, spawner2d, Map);
                    List<Vector2Int> pathSegment = paths[i];
                    if (combinedPath.Count > 0 && pathSegment.Count > 0)
                    {
                        // Remove the first point of the next segment if it's the same as the last point of the previous segment to avoid duplicate points
                        if (combinedPath[combinedPath.Count - 1] == pathSegment[0])
                        {
                            pathSegment.RemoveAt(0);
                        }
                    }
                    combinedPath.AddRange(pathSegment);
                }



            }



            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i] != null && paths[i].Count > 0)
                {
                    Debug.Log("Path does exists");

                    if (i == paths.Length - 1)
                    {

                        return true;
                    }

                }


            }

            return false;
        }


    }

    Vector2Int calculateGridPosition(Vector3 worldPosition, float tileSize, Vector3 currentTransformPosition)
    {
        int i = Mathf.FloorToInt((worldPosition.x - currentTransformPosition.x) / tileSize);
        int j = Mathf.FloorToInt((worldPosition.z - currentTransformPosition.z) / tileSize);

        return new Vector2Int(i, j);
    }

    static int[,] RemoveWallFromRealWorldPosition(int[,] grid, Vector3 antSpawnerWorldPosition, float tileSize, int ObjectValue, Vector3 currentTransformPosition)
    {
        // Calculate grid coordinates from antSpawnerWorldPosition
        int i = Mathf.FloorToInt((antSpawnerWorldPosition.x - currentTransformPosition.x) / tileSize);
        int j = Mathf.FloorToInt((antSpawnerWorldPosition.z - currentTransformPosition.z) / tileSize);

        // Ensure the calculated indices are within the bounds of the grid
        if (i >= 0 && i < grid.GetLength(0) && j >= 0 && j < grid.GetLength(1))
        {
            // Set the grid value to 3 at the calculated coordinates
            grid[i, j] = ObjectValue;
        }
        else
        {
            Debug.LogWarning("Ant spawner position is out of the grid bounds.");
        }

        return grid;
    }

    static int[,] RemoveWallsFromRealWorldPosition(int[,] grid, List<Vector3> antSpawnerWorldPositions, float tileSize, int ObjectValue, Vector3 currentTransformPosition)
    {
        foreach (Vector3 antSpawnerWorldPosition in antSpawnerWorldPositions)
        {
            grid = RemoveWallFromRealWorldPosition(grid, antSpawnerWorldPosition, tileSize, ObjectValue, currentTransformPosition);
        }
        return grid;
    }


    static int[,] applyCellularAutomaton(int[,] grid, int count, int erosionLimit)
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
                                    if (tempGrid[y, x] == 1)
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
                    if (FloorOrWall(grid[j, k]))
                    {
                        grid[j, k] = neighborWallCount > erosionLimit ? 1 : 0; // Update based on neighbor count
                    }
                }
            }
        }
        return grid;
    }


}