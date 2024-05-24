using System.Collections.Generic;
using UnityEngine;

public class WebView : MonoBehaviour, IScreenView
{
    // Object Pooler
    public ObjectPooler objectPooler;

    // Game Objects prefabs
    public GameObject wallPrefab;
    public GameObject tilePrefab;
    public GameObject checkPointPrefab;
    public GameObject spawnPointPrefab;
    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject entrancePrefab;
    public GameObject linePrefab;

    public Material clusterMaterial;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;

    public Material pathMaterial;


    // Tags
    private string wallTag = "Wall";
    private string tileTag = "Tile";
    private string checkPointTag = "CheckPoint";
    private string spawnPointTag = "SpawnPoint";
    private string agentTag = "Agent";
    private string nodeTag = "Node";
    private string entranceTag = "Entrance";
    private string lineTag = "Line";



    // Local variables
    private GameObject[,] InstantiatedMap;
    private List<GameObject> InstantiatedGraph;
    private readonly float tileScale = 0.1f;
    private readonly int tileSize = 1;
    private readonly float nodeScale = 0.1f;


    private ScreenViewModel screenViewModel;
    // Presenter
    private ScreenPresenter screenPresenter;

    // GameManager
    private MyGameManager myGameManager;

    void Awake()
    {

        myGameManager = new();
        screenPresenter = new(myGameManager);
        screenViewModel = screenPresenter.PackageData();
        int mapSize = screenViewModel.map.GetLength(0) * screenViewModel.map.GetLength(1);
        InstantiatedGraph = new List<GameObject>();

        Debug.Log("Map Size: " + mapSize);
        // Add pools programmatically
        if (objectPooler == null)
        {
            Debug.Log("objectPooler is null");
        }
        if (wallPrefab == null)
        {
            Debug.Log("wallPrefab is null");
        }
        if (wallTag == null)
        {
            Debug.Log("wallTag is null");
        }

        objectPooler.AddPool(wallTag, wallPrefab, mapSize);
        objectPooler.AddPool(tileTag, tilePrefab, mapSize);
        objectPooler.AddPool(checkPointTag, checkPointPrefab, 50);
        objectPooler.AddPool(spawnPointTag, spawnPointPrefab, 5);
        objectPooler.AddPool(agentTag, agentPrefab, 10);
        objectPooler.AddPool(nodeTag, nodePrefab, mapSize);
        objectPooler.AddPool(entranceTag, entrancePrefab, mapSize / 10);
        objectPooler.AddPool(lineTag, linePrefab, mapSize * 5);
    }

    void Start()
    {

        RenderMap(screenViewModel);
        RenderGraph(1, screenViewModel);

        // myGameManager.graphController.Preprocessing(3);
        Vector2Int start = screenViewModel.checkPoints[0].ArrayPosition;
        Vector2Int end = screenViewModel.spawnPoint.ArrayPosition;
        HPAPath path = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 2);

        DrawPath(path);


    }

    void Update()
    {

    }

    public void RenderMap(ScreenViewModel screenViewModel)
    {
        MapObject[,] map = screenViewModel.map;
        List<CheckPoint> checkPoints = screenViewModel.checkPoints;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;

        ClearMap(InstantiatedMap);
        InstantiatedMap = new GameObject[map.GetLength(1), map.GetLength(0)];
        InstantiateMap(map);
        InstantiateCheckPoints(checkPoints);
        InstantiateSpawnPoint(spawnPoint);

    }
    private void InstantiateMap(MapObject[,] map)
    {
        int mapWidth = (int)(map.GetLength(0) * tileScale);
        int mapHeight = (int)(map.GetLength(1) * tileScale);
        Vector3Int position = new Vector3Int(map.GetLength(0) / 2, 0, map.GetLength(1) / 2);
        GameObject floor = Instantiate(tilePrefab, position, Quaternion.identity);
        floor.transform.position = position;
        floor.transform.localScale = new Vector3(mapWidth, 0, mapHeight);

        foreach (MapObject mapObject in map)
        {
            Vector3Int worldPosition = ConvertVector2DTo3D(mapObject.ArrayPosition);
            int i = mapObject.ArrayPosition.x;
            int j = mapObject.ArrayPosition.y;
            if (mapObject.Type == MapObject.ObjectType.Wall)
            {
                InstantiatedMap[i, j] = Instantiate(wallPrefab, worldPosition, Quaternion.identity);
            }
        }
    }

    private void InstantiateCheckPoints(List<CheckPoint> checkPoints)
    {
        foreach (CheckPoint checkPoint in checkPoints)
        {
            Vector3Int worldPosition = ConvertVector2DTo3D(checkPoint.ArrayPosition);
            int i = checkPoint.ArrayPosition.x;
            int j = checkPoint.ArrayPosition.y;
            InstantiatedMap[i, j] = objectPooler.SpawnFromPool(checkPointTag, worldPosition, Quaternion.identity);
        }
    }

    private void InstantiateSpawnPoint(AgentSpawnPoint spawnPoint)
    {
        Vector3Int worldPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition);
        int i = spawnPoint.ArrayPosition.x;
        int j = spawnPoint.ArrayPosition.y;
        InstantiatedMap[i, j] = objectPooler.SpawnFromPool(spawnPointTag, worldPosition, Quaternion.identity);
    }

    private Vector3Int ConvertVector2DTo3D(Vector2Int arrayPosition)
    {
        return new Vector3Int(arrayPosition.x, 0, arrayPosition.y);
    }

    private void ClearMap(GameObject[,] instantiatedMap)
    {
        if (instantiatedMap == null) return;
        foreach (GameObject mapObject in instantiatedMap)
        {
            if (mapObject != null)
            {
                mapObject.SetActive(false);
            }
        }
    }

    private void RenderGraph(int level, ScreenViewModel screenViewModel)
    {
        ClearGraph(InstantiatedGraph);
        InstantiatedGraph = new List<GameObject>();
        IGraphModel graph = screenViewModel.graph;
        if (graph.ClusterByLevel.TryGetValue(level, out var clusters))
        {
            foreach (Cluster cluster in clusters)
            {
                Vector3 bottomLeft = transform.position + new Vector3(cluster.bottomLeftPos.x * tileSize, 0, cluster.bottomLeftPos.y * tileSize);
                Vector3 topRight = transform.position + new Vector3(cluster.topRightPos.x * tileSize, 0, cluster.topRightPos.y * tileSize);
                Vector3 topLeft = transform.position + new Vector3(cluster.bottomLeftPos.x * tileSize, 0, cluster.topRightPos.y * tileSize);
                Vector3 bottomRight = transform.position + new Vector3(cluster.topRightPos.x * tileSize, 0, cluster.bottomLeftPos.y * tileSize);
                DrawLine(bottomLeft, topLeft, clusterMaterial);
                DrawLine(topLeft, topRight, clusterMaterial);
                DrawLine(topRight, bottomRight, clusterMaterial);
                DrawLine(bottomRight, bottomLeft, clusterMaterial);
                DrawEntrances(cluster);
                foreach (var node in cluster.Nodes)
                {
                    DrawNode(node);
                    DrawEdges(node, HPAEdgeType.INTRA);
                    DrawEdges(node, HPAEdgeType.INTER);
                }
            }
        }
        else
        {
            Debug.Log($"No data available for level {level}");
        }
    }

    private void ClearGraph(List<GameObject> instantiatedGraph)
    {
        if (instantiatedGraph == null) return;
        foreach (GameObject graphObject in instantiatedGraph)
        {
            graphObject.SetActive(false);
        }
    }

    private void DrawNode(HPANode node)
    {
        GameObject nodeObj = objectPooler.SpawnFromPool(nodeTag, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        InstantiatedGraph.Add(nodeObj);
    }

    private void DrawPath(HPAPath path)
    {
        for (int i = 0; i < path.path.Count - 1; i++)
        {
            Vector3 start = transform.position + new Vector3(path.path[i].Position.x * tileSize, -0.8f, path.path[i].Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(path.path[i + 1].Position.x * tileSize, -0.8f, path.path[i + 1].Position.y * tileSize);

            if (start == null)
            {
                Debug.Log("Start is null");
            }
            if (end == null)
            {
                Debug.Log("End is null");
            }
            if (pathMaterial == null)
            {
                Debug.Log("intraEdgeMaterial is null");
            }
            DrawLine(start, end, pathMaterial);
        }
    }

    private void DrawEntrances(Cluster cluster)
    {
        foreach (Entrance entrance in cluster.Entrances)
        {
            GameObject entranceObj = objectPooler.SpawnFromPool(entranceTag, transform.position + new Vector3(entrance.Node1.Position.x * tileSize, 0, entrance.Node1.Position.y * tileSize), Quaternion.identity);
            entranceObj.transform.localScale = Vector3.one * nodeScale * tileSize;
            InstantiatedGraph.Add(entranceObj);
        }
    }

    private void DrawEdges(HPANode node, HPAEdgeType edgeType)
    {
        foreach (HPAEdge edge in node.Edges)
        {
            if (edge.Type != edgeType) continue;
            Vector3 start = transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(edge.Node2.Position.x * tileSize, 0, edge.Node2.Position.y * tileSize);
            DrawLine(start, end, edgeType == HPAEdgeType.INTRA ? intraEdgeMaterial : interEdgeMaterial);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Material material)
    {
        GameObject lineObj = objectPooler.SpawnFromPool(lineTag, Vector3.zero, Quaternion.identity);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = 0.05f * tileSize;
        lr.endWidth = 0.05f * tileSize;
        lr.SetPositions(new Vector3[] { start, end });
        InstantiatedGraph.Add(lineObj);
    }
}
