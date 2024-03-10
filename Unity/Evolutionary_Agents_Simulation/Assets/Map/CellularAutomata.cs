using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CellularAutomata : MonoBehaviour
{


    public GameObject MapWall;
    public GameObject Plane;
    public GameObject CheckPoint;

    public GameObject pathVisualizer; // Assign this in the Unity Editor

    public GameObject Spawner;







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

    private MapModel mapModel;


    void Start()
    {
        mapModel = Resources.Load<MapModel>("MapModel");

        if (mapModel.MapNumber != 0)
        {
            InitializePreDefinedParameters(mapModel.MapNumber);
        }
        else
        {
            InitializeParameters();
        }
        MapController.CreateMap(
            ref spawnedObjects, // ref parameter must be in the correct position
            mapModel: mapModel,
            MapWall: MapWall,
            CheckPoint: CheckPoint,
            currentTransformPosition: transform.position,
            antSpawnerWorldPositions: Spawner.GetComponent<ObjectDropper>().colonyPositions
            );
    }

    void Update()
    {
        if (ParametersChanged())
        {
            Debug.Log("Paramters changed");
            MapController.ClearMap(ref spawnedObjects);
            InitializeParameters();
            MapController.CreateMap(
                ref spawnedObjects, // ref parameter must be in the correct position
                mapModel: mapModel,
                MapWall: MapWall,
                CheckPoint: CheckPoint,
                currentTransformPosition: transform.position,
                antSpawnerWorldPositions: Spawner.GetComponent<ObjectDropper>().colonyPositions
                );
        }
    }

    public static void ViewDestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public static GameObject ViewInistiateObject(GameObject prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.identity);
    }
    // Coroutine to move the pathVisualizer along the path
    IEnumerator FollowPath(List<Vector2Int> path, Vector3 gridOrigin, float tileSize)
    {
        foreach (Vector2Int cell in path)
        {
            Vector3 worldPosition = gridOrigin + new Vector3(cell.x * tileSize, 1, cell.y * tileSize) + new Vector3(tileSize / 2.0f, 0, tileSize / 2.0f);
            pathVisualizer.transform.position = worldPosition;
            yield return new WaitForSeconds(0.01f); // Adjust for visual effect
        }
    }

    // Method to start path visualization
    public void StartVisualizingPath(List<Vector2Int> path, Vector3 gridOrigin, float tileSize)
    {
        StartCoroutine(FollowPath(path, gridOrigin, tileSize));
    }

    void InitializeParameters()
    {
        UnityEngine.Random.InitState(mapModel.RandomSeed);

        mapModel.mapSize = (int)(Plane.transform.localScale.x * Plane.transform.localScale.x);
        MapWall.transform.localScale = new Vector3(mapModel.TileSize, MapWall.transform.localScale.y, mapModel.TileSize);

        // Update last known parameter values for tracking changes
        lastDensity = mapModel.Density;
        lastNumberOfCheckPoints = mapModel.NumberOfCheckPoints;
        lastTileSize = mapModel.TileSize;
        lastCellularIterations = mapModel.CellularIterations;
        lastCheckPointSpacing = mapModel.CheckPointSpacing;
        lastErosionLimit = mapModel.ErosionLimit;
        lastRandomSeed = mapModel.RandomSeed; // Ensure this line is added
        lastColonyCoordinates = new List<Vector3>(Spawner.GetComponent<ObjectDropper>().colonyPositions);


    }

    void InitializePreDefinedParameters(int MapNumber)
    {
        Dictionary<string, int> mapParameters = InitCustomMaps.GetMapParameters(MapNumber);

        mapModel.Density = mapParameters["Density"];
        mapModel.NumberOfCheckPoints = mapParameters["NumberOfCheckPoints"];
        mapModel.TileSize = mapParameters["TileSize"];
        mapModel.CellularIterations = mapParameters["CellularIterations"];
        mapModel.CheckPointSpacing = mapParameters["CheckPointSpacing"];
        mapModel.ErosionLimit = mapParameters["ErosionLimit"];
        mapModel.RandomSeed = mapParameters["RandomSeed"];




    }

    int[,] visualizePath(int[,] map, List<Vector2Int> path)
    {
        Debug.Log("Visualizing path with path length: " + path.Count);

        StartVisualizingPath(path, transform.position, mapModel.TileSize);


        // foreach (Vector2Int cell in path)
        // {
        //     Debug.Log(cell.x);
        //     Debug.Log(cell.y);
        //     //   map[cell.x, cell.y] = 0;
        // }
        // return map;
        return map;
    }

    bool ParametersChanged()
    {
        // Debug.Log("Last density:" + lastDensity.ToString() + " map model: " + mapModel.Density.ToString());


        // Debug.Log("Last number of check points: " + lastNumberOfCheckPoints.ToString() + " map model: " + mapModel.NumberOfCheckPoints.ToString());

        // Debug.Log("Last tile size: " + lastTileSize.ToString() + " map model: " + mapModel.TileSize.ToString());

        // Debug.Log("Last cellular iterations: " + lastCellularIterations.ToString() + " map model: " + mapModel.CellularIterations.ToString());

        // Debug.Log("Last check point spacing: " + lastCheckPointSpacing.ToString() + " map model: " + mapModel.CheckPointSpacing.ToString());

        // Debug.Log("Last erosion limit: " + lastErosionLimit.ToString() + " map model: " + mapModel.ErosionLimit.ToString());

        // Debug.Log("Last random seed: " + lastRandomSeed.ToString() + " map model: " + mapModel.RandomSeed.ToString());







        if (lastDensity != mapModel.Density ||
            lastNumberOfCheckPoints != mapModel.NumberOfCheckPoints ||
            lastTileSize != mapModel.TileSize ||
            lastCellularIterations != mapModel.CellularIterations ||
            lastCheckPointSpacing != mapModel.CheckPointSpacing ||
            lastErosionLimit != mapModel.ErosionLimit ||
            lastRandomSeed != mapModel.RandomSeed)
        {
            Debug.Log("FIRST PARAMETERS CHANGED! !!!  ");
            return true;
        }

        var currentColonyPositions = Spawner.GetComponent<ObjectDropper>().colonyPositions;
        if (lastColonyCoordinates == null || currentColonyPositions == null)
        {
            Debug.Log("Colony positions are null");

            // Debug.Log("lastColonyCoordinates is null" + lastColonyCoordinates == null);
            // Debug.Log("currentColonyPositions is null" + currentColonyPositions == null);

            return lastColonyCoordinates != currentColonyPositions;
        }

        if (lastColonyCoordinates.Count != currentColonyPositions.Count)
        {
            Debug.Log("Colony positions count is different");

            return true;
        }

        for (int i = 0; i < lastColonyCoordinates.Count; i++)
        {
            if (lastColonyCoordinates[i] != currentColonyPositions[i])
            {
                Debug.Log("Colony positions are different in for loop");

                return true;
            }
        }

        return false;
    }





}

