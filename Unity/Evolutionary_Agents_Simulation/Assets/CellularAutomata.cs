using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class CellularAutomata : MonoBehaviour
{

    Astar aStar = new Astar();
    public GameObject MapWall;
    public GameObject Plane;
    public GameObject CheckPoint;

    public GameObject Spawner;

    private int MapSize;
    [Range(0, 20)] public int NumberOfCheckPoints;
    [Range(1, 10)] public int tileSize;
    [Range(0, 100)] public float density; // Density field to control noise
    [Range(0, 100)] public int CellularIterations;
    [Range(1, 30)] public int CheckPointSpacing;
    [Range(1, 10)] public int erosionLimit = 4;
    public int randomSeed = 42; // Added seed for random number generator

    private List<Vector3> ColonyCoordinates;



    private int mapTileAmount;
    private List<GameObject> spawnedObjects = new List<GameObject>();

    // Variables to track if any parameter has changed
    private float lastDensity;
    private int lastNumberOfCheckPoints;
    private int lastTileSize;
    private int lastCellularIterations;
    private int lastCheckPointSpacing;
    private int lastErosionLimit;
    private int lastRandomSeed; // Track the last seed used


    private List<Vector3> lastColonyCoordinates;


    void Start()
    {
        InitializeParameters();
        CreateMap();



    }

    void Update()
    {
        if (ParametersChanged())
        {
            Debug.Log("Paramters changed");
            ClearMap();
            InitializeParameters();
            CreateMap();
        }
    }

    void InitializeParameters()
    {
        UnityEngine.Random.InitState(randomSeed);

        MapSize = (int)(Plane.transform.localScale.x * Plane.transform.localScale.x);
        MapWall.transform.localScale = new Vector3(tileSize, MapWall.transform.localScale.y, tileSize);

        // Update last known parameter values for tracking changes
        lastDensity = density;
        lastNumberOfCheckPoints = NumberOfCheckPoints;
        lastTileSize = tileSize;
        lastCellularIterations = CellularIterations;
        lastCheckPointSpacing = CheckPointSpacing;
        lastErosionLimit = erosionLimit;
        lastRandomSeed = randomSeed; // Ensure this line is added
        lastColonyCoordinates = Spawner.GetComponent<ObjectDropper>().colonyPositions;


    }

    int[,] visualizePath(int[,] map, List<Vector2Int> path)
    {
        foreach (Vector2Int cell in path)
        {
            Debug.Log(cell.x);
            Debug.Log(cell.y);
            map[cell.x, cell.y] = 2;
        }
        return map;
    }

    bool ParametersChanged()
    {
        return lastDensity != density ||
               lastNumberOfCheckPoints != NumberOfCheckPoints ||
               lastTileSize != tileSize ||
               lastCellularIterations != CellularIterations ||
               lastCheckPointSpacing != CheckPointSpacing ||
               lastErosionLimit != erosionLimit ||
               lastRandomSeed != randomSeed // Include the seed in the change check 
                || !lastColonyCoordinates.Equals(Spawner.GetComponent<ObjectDropper>().colonyPositions);

    }

    void ClearMap()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    void CreateMap()
    {
        mapTileAmount = MapSize / tileSize;
        int[,] Map = init2dMap(MapSize, tileSize, density, CellularIterations);
        for (int i = 0; i < mapTileAmount; i++)
        {
            for (int j = 0; j < mapTileAmount; j++)
            {
                Vector3 position = transform.position + new Vector3(i * tileSize, 0, j * tileSize) + new Vector3((float)(tileSize / 2.0), 0, (float)(tileSize / 2.0));
                if (Map[i, j] == 1) // 1 represents wall
                {
                    GameObject wall = Instantiate(MapWall, position, Quaternion.identity);
                    spawnedObjects.Add(wall);
                }
                else if (Map[i, j] == 2) // 2 represents checkpoint
                {
                    GameObject checkpoint = Instantiate(CheckPoint, position, Quaternion.identity);
                    spawnedObjects.Add(checkpoint);
                }
                else if (Map[i, j] == 3)
                {
                    // Spawned already made. 

                }
            }
        }
    }




    int[,] init2dMap(int MapSize, int tileSize, float density, int iterations)
    {
        mapTileAmount = MapSize / tileSize;
        int[,] Map = new int[mapTileAmount, mapTileAmount];
        Map = generateCheckpoints(Map, NumberOfCheckPoints);
        //      int[,] checkpointCoordinates = getObjectCoordinates(Map, NumberOfCheckPoints, 2);


        Map = RemoveWallsFromRealWorldPosition(Map, Spawner.transform.position, tileSize, 3);

        Map = generateNoise(Map, density);
        Map = applyCellularAutomaton(Map, iterations, erosionLimit);
        //  Map = checkIfpathExists(Map);





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

    int[,] getObjectCoordinates(int[,] grid, int numberOfCheckPoints, int ObjectValue)
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
            }
        }
        return CheckPointCoordinates;
    }

    int[,] checkIfpathExists(int[,] Map)
    {
        Boolean pathExists = false;
        while (!pathExists)
        {
            int[,] checkpoints = getObjectCoordinates(Map, NumberOfCheckPoints, 2);
            int[,] spawnerCoordinate = getObjectCoordinates(Map, 1, 3);

            Boolean[] paths = new Boolean[checkpoints.Length];


            for (int i = 0; i < checkpoints.GetLength(0); i++)
            {
                foreach (Vector3 SpawnerPosition in lastColonyCoordinates)
                {
                    Vector2Int spawner2d = calculateGridPosition(SpawnerPosition, tileSize);

                    Vector2Int tempCheckPoint = new Vector2Int(checkpoints[i, 0], checkpoints[i, 1]);
                    paths[i] = aStar.FindPath(spawner2d, tempCheckPoint, Map);
                }

            }

            for (int i = 0; i < paths.Length; i++)
            {
                bool path = paths[i];
                if (!path)
                {
                    CellularIterations += 1;
                    Map = applyCellularAutomaton(Map, CellularIterations, erosionLimit);


                }
                else
                {
                    Debug.Log("Path exists");
                }

                if (i == paths.Length - 1 && path == true)
                {
                    pathExists = true;
                }

            }

        }
        return Map;
    }

    Vector2Int calculateGridPosition(Vector3 worldPosition, float tileSize)
    {
        int i = Mathf.FloorToInt((worldPosition.x - transform.position.x) / tileSize);
        int j = Mathf.FloorToInt((worldPosition.z - transform.position.z) / tileSize);

        return new Vector2Int(i, j);
    }

    int[,] RemoveWallsFromRealWorldPosition(int[,] grid, Vector3 antSpawnerWorldPosition, float tileSize, int ObjectValue)
    {
        // Calculate grid coordinates from antSpawnerWorldPosition
        int i = Mathf.FloorToInt((antSpawnerWorldPosition.x - transform.position.x) / tileSize);
        int j = Mathf.FloorToInt((antSpawnerWorldPosition.z - transform.position.z) / tileSize);

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

