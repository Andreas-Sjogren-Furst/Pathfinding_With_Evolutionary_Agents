// written by: Gustav Clausen s214940

using System.Collections.Generic;
using PlasticGui.Help.Conditions;
using UnityEngine;

public class WebView : MonoBehaviour, IScreenView
{

    // MMAS materials
    public Material edgeMaterial; // Assign a material for the edges
    public Material tourMaterial; // Assign a material for highlighting the tos

    public static WebView Instance { get; private set; }
    // Object Pooler

    // Game Objects prefabs
    public GameObject wallPrefab;
    public GameObject tilePrefab;
    public GameObject checkPointPrefab;
    public GameObject spawnPointPrefab;
    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject entrancePrefab;
    public GameObject linePrefab;
    public GameObject frontierPrefab;

    public Material clusterMaterial;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;
    public Material darkColor;
    public Material whiteColor;

    public Material pathMaterial;


    // Local variables
    private GameObject[,] InstantiatedMap;
    private List<GameObject> InstantiatedCheckPoints;
    private GameObject InstantiatedSpawnPoint;
    private List<GameObject> pHPAGraph;
    private List<GameObject>[] InstantiatedHPAGraphs;
    private List<GameObject> InstantiatedAgents;
    private List<GameObject> InstantiatedMMASNodes;
    private List<GameObject> InstantiatedMMASEdges;
    private List<GameObject> InstantiatedFrontiers;
    private List<GameObject> InstantiatedPath;
    private readonly float tileScale = 0.1f;
    private readonly int tileSize = 1;
    private readonly float nodeScale = 0.1f;
    private readonly float animationSpeed = 10f;
    public int amountHPALevels;
    public int currentHPALevel;

    // Presenter
    public ScreenPresenter screenPresenter;

    // GameManager
    public MyGameManager myGameManager;

    void Awake()
    {
        Instance = this;
        myGameManager = new();
        screenPresenter = new(myGameManager);
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        amountHPALevels = myGameManager.HPAGraphController._graphModel.ClusterByLevel.Count;
        InstantiatedHPAGraphs = new List<GameObject>[amountHPALevels];
        for (int i = 0; i < amountHPALevels; i++) { InstantiatedHPAGraphs[i] = new(); }
        InstantiatedCheckPoints = new();
        InstantiatedAgents = new();
        InstantiatedFrontiers = new();
        InstantiatedMap = new GameObject[screenViewModel.map.GetLength(1), screenViewModel.map.GetLength(0)];
        InstantiatedMMASEdges = new();
        InstantiatedMMASNodes = new();
        InstantiatedPath = new();
        pHPAGraph = InstantiatedHPAGraphs[0];
        currentHPALevel = 1;
    }
    void Start()
    {
        RenderMap();
        RenderHPAGraphs();
        SpawnAgents();
        RenderMMASGraph();
        RenderFrontiers();
        RenderPath();





        //myGameManager.graphController.Preprocessing(3);

        // Vector2Int start = new(26, 40);
        // Vector2Int end = new(44, 28);

        // HPAPath path = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 2);

        // (List<Vector2Int> path1, int nodesExplored) = Astar.FindPath(start, end, CellularAutomata.Convert3DTo2D(screenPresenter.PackageData().map));

        /*ScreenViewModel screenViewModel = screenPresenter.PackageData();
        if (screenViewModel.checkPoints.Count > 1)
        {
            Vector2Int start = screenViewModel.spawnPoint.ArrayPosition;
            Vector2Int end = screenViewModel.checkPoints[0].ArrayPosition;
            Debug.Log("Start: " + start);
            Debug.Log("End: " + end);
            Debug.Log("Level: " + currentHPALevel);
            if (myGameManager == null)
            {
                Debug.Log("GameManager is null");
            }
            HPAPath path = myGameManager.HPAGraphController.HierarchicalSearch(start, end, currentHPALevel);

            if (path != null && path.path.Count > 0)
            {
                Debug.Log("Path Length: " + path.path.Count);
                DrawPath(path, InstantiatedMMASEdges); // TODO: Change to InstantiatedPath, and create UI button for it. 
            }
        }*/






    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            myGameManager.agentController.SimulateAgents();
            MoveAgents();
            RenderFrontiers();
        }
    }

    public void CreateNewMap(MapModel mapModel)
    {
        DestroyAllObjects();
        myGameManager = new(mapModel);
        InitVariables(myGameManager);
        Start();
    }

    private void DestroyAllObjects()
    {
        foreach (GameObject gameObject in InstantiatedMap)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedCheckPoints)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedCheckPoints)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in pHPAGraph)
        {
            Destroy(gameObject);
        }
        foreach (List<GameObject> listOfGameObjects in InstantiatedHPAGraphs)
        {
            foreach (GameObject gameObject in listOfGameObjects) Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedAgents)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedMMASNodes)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedMMASEdges)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedFrontiers)
        {
            Destroy(gameObject);
        }
        foreach (GameObject gameObject in InstantiatedPath)
        {
            Destroy(gameObject);
        }
    }
    private void InitVariables(MyGameManager myGameManager)
    {
        screenPresenter = new(myGameManager);
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        amountHPALevels = myGameManager.HPAGraphController._graphModel.ClusterByLevel.Count;
        InstantiatedHPAGraphs = new List<GameObject>[amountHPALevels];
        for (int i = 0; i < amountHPALevels; i++) { InstantiatedHPAGraphs[i] = new(); }
        InstantiatedCheckPoints = new();
        InstantiatedAgents = new();
        InstantiatedFrontiers = new();
        InstantiatedMap = new GameObject[screenViewModel.map.GetLength(1), screenViewModel.map.GetLength(0)];
        InstantiatedMMASEdges = new();
        InstantiatedMMASNodes = new();
        InstantiatedPath = new();
        pHPAGraph = InstantiatedHPAGraphs[0];
        currentHPALevel = 1;
    }
    public void RenderMap()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        MapObject[,] map = screenViewModel.map;
        List<CheckPoint> checkPoints = screenViewModel.checkPoints;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;

        ClearMap();
        InstantiateMap(map);
        InstantiateCheckPoints(checkPoints);
        InstantiateSpawnPoint(spawnPoint);

    }
    private void InstantiateMap(MapObject[,] map)
    {
        foreach (MapObject mapObject in map)
        {
            Vector3Int worldPosition = ConvertVector2DTo3D(mapObject.ArrayPosition);
            int i = mapObject.ArrayPosition.x;
            int j = mapObject.ArrayPosition.y;
            if (mapObject.Type == MapObject.ObjectType.Tile)
                InstantiatedMap[i, j] = Instantiate(tilePrefab, worldPosition, Quaternion.identity);
            else
                InstantiatedMap[i, j] = Instantiate(wallPrefab, worldPosition, Quaternion.identity);
        }
    }

    private void InstantiateCheckPoints(List<CheckPoint> checkPoints)
    {
        foreach (CheckPoint checkPoint in checkPoints)
        {
            Vector3Int worldPosition = ConvertVector2DTo3D(checkPoint.ArrayPosition);
            int i = checkPoint.ArrayPosition.x;
            int j = checkPoint.ArrayPosition.y;
            InstantiatedCheckPoints.Add(Instantiate(checkPointPrefab, worldPosition, Quaternion.identity));
        }
    }

    private void InstantiateSpawnPoint(AgentSpawnPoint spawnPoint)
    {
        Vector3Int worldPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition);
        int i = spawnPoint.ArrayPosition.x;
        int j = spawnPoint.ArrayPosition.y;
        InstantiatedSpawnPoint = Instantiate(spawnPointPrefab, worldPosition, Quaternion.identity);
    }

    private Vector3Int ConvertVector2DTo3D(Vector2Int arrayPosition)
    {
        return new Vector3Int(arrayPosition.x, 0, arrayPosition.y);
    }

    public void ClearMap()
    {
        if (InstantiatedMap == null) return;
        foreach (GameObject mapObject in InstantiatedMap)
            if (mapObject != null) Destroy(mapObject);

        foreach (GameObject checkPoint in InstantiatedCheckPoints)
            if (checkPoint != null) Destroy(checkPoint);

        if (InstantiatedSpawnPoint != null)
            Destroy(InstantiatedSpawnPoint);
    }

    public void ShowOrHideMap(bool isOn)
    {
        foreach (GameObject mapObject in InstantiatedMap)
            if (mapObject != null) mapObject.SetActive(isOn);

        foreach (GameObject checkPoint in InstantiatedCheckPoints)
            if (checkPoint != null) checkPoint.SetActive(isOn);

        if (InstantiatedSpawnPoint != null)
            InstantiatedSpawnPoint.SetActive(isOn);
    }

    // ### Webview for HPA Graph ###
    private void RenderHPAGraphs()
    {

        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        IGraphModel graph = screenViewModel.hpaGraph;
        foreach (KeyValuePair<int, HashSet<Cluster>> hpaGraph in graph.ClusterByLevel)
        {
            if (graph.ClusterByLevel.TryGetValue(hpaGraph.Key, out var clusters))
            {
                int arrayIndex = hpaGraph.Key - 1;
                foreach (Cluster cluster in clusters)
                {
                    Vector3 bottomLeft = transform.position + new Vector3(cluster.bottomLeftPos.x * tileSize, 0, cluster.bottomLeftPos.y * tileSize);
                    Vector3 topRight = transform.position + new Vector3(cluster.topRightPos.x * tileSize, 0, cluster.topRightPos.y * tileSize);
                    Vector3 topLeft = transform.position + new Vector3(cluster.bottomLeftPos.x * tileSize, 0, cluster.topRightPos.y * tileSize);
                    Vector3 bottomRight = transform.position + new Vector3(cluster.topRightPos.x * tileSize, 0, cluster.bottomLeftPos.y * tileSize);
                    DrawLine(bottomLeft, topLeft, clusterMaterial, InstantiatedHPAGraphs[arrayIndex]);
                    DrawLine(topLeft, topRight, clusterMaterial, InstantiatedHPAGraphs[arrayIndex]);
                    DrawLine(topRight, bottomRight, clusterMaterial, InstantiatedHPAGraphs[arrayIndex]);
                    DrawLine(bottomRight, bottomLeft, clusterMaterial, InstantiatedHPAGraphs[arrayIndex]);
                    DrawEntrances(cluster, InstantiatedHPAGraphs[arrayIndex]);
                    foreach (var node in cluster.Nodes)
                    {
                        DrawNode(node, InstantiatedHPAGraphs[arrayIndex]);
                        DrawEdges(node, HPAEdgeType.INTRA, InstantiatedHPAGraphs[arrayIndex]);
                        DrawEdges(node, HPAEdgeType.INTER, InstantiatedHPAGraphs[arrayIndex]);
                    }
                }
            }
            else
            {
                Debug.Log($"No data available for level {hpaGraph.Key}");
            }
        }
    }

    private void ClearHPAGraph(List<GameObject> instantiatedGraph)
    {
        if (instantiatedGraph == null) return;
        foreach (GameObject graphObject in instantiatedGraph)
        {
            Destroy(graphObject);
        }
    }

    public void ShowOrHideHPAGraph(bool isOn)
    {
        foreach (GameObject graphObject in pHPAGraph)
            if (graphObject != null) graphObject.SetActive(isOn);
    }

    public void SetCurrentHPALevel(int level)
    {
        pHPAGraph = InstantiatedHPAGraphs[level];
        currentHPALevel = level + 1;
    }

    private void DrawNode(HPANode node, List<GameObject> InstantiatedGraph)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        InstantiatedGraph.Add(nodeObj);
        nodeObj.SetActive(false);
    }
    public void ShowOrHidePath(bool isOn)
    {
        foreach (GameObject pathObject in InstantiatedPath)
        {
            pathObject.SetActive(isOn);
        }
    }
    public void RenderPath()
    {
        ClearPath();
        InstantiatedPath = new();
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        if (screenViewModel.checkPoints.Count <= 0) return;
        Vector2Int start = screenViewModel.spawnPoint.ArrayPosition;
        Vector2Int end = screenViewModel.checkPoints[0].ArrayPosition;
        HPAPath path = myGameManager.HPAGraphController.HierarchicalSearch(start, end, currentHPALevel);
        if (path == null && path.path.Count <= 0) return;
        DrawPath(path, InstantiatedPath);
    }
    private void ClearPath()
    {
        foreach (GameObject pathObject in InstantiatedPath)
        {
            Destroy(pathObject);
        }
    }
    private void DrawPath(HPAPath path, List<GameObject> InstantiatedGraph)
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
            DrawLine(start, end, pathMaterial, InstantiatedGraph);
        }
    }


    private void DrawPath(List<Vector2Int> path, List<GameObject> InstantiatedGraph)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            Vector3 start = transform.position + new Vector3(path[i].x * tileSize, -0.8f, path[i].y * tileSize);
            Vector3 end = transform.position + new Vector3(path[i + 1].x * tileSize, -0.8f, path[i + 1].y * tileSize);

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
            DrawLine(start, end, pathMaterial, InstantiatedGraph);
        }
    }

    private void DrawEntrances(Cluster cluster, List<GameObject> InstantiatedGraph)
    {
        foreach (Entrance entrance in cluster.Entrances)
        {
            GameObject entranceObj = Instantiate(entrancePrefab, transform.position + new Vector3(entrance.Node1.Position.x * tileSize, 0, entrance.Node1.Position.y * tileSize), Quaternion.identity);
            entranceObj.transform.localScale = Vector3.one * nodeScale * tileSize;
            InstantiatedGraph.Add(entranceObj);
            entranceObj.SetActive(false);
        }
    }

    private void DrawEdges(HPANode node, HPAEdgeType edgeType, List<GameObject> InstantiatedGraph)
    {
        foreach (HPAEdge edge in node.Edges)
        {
            if (edge.Type != edgeType) continue;
            Vector3 start = transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(edge.Node2.Position.x * tileSize, 0, edge.Node2.Position.y * tileSize);
            DrawLine(start, end, edgeType == HPAEdgeType.INTRA ? intraEdgeMaterial : interEdgeMaterial, InstantiatedGraph);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Material material, List<GameObject> InstantiatedGraph)
    {
        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = 0.05f * tileSize * 5;
        lr.endWidth = 0.05f * tileSize * 5;
        lr.SetPositions(new Vector3[] { start, end });
        InstantiatedGraph.Add(lineObj);
        lineObj.SetActive(false);
    }

    // ### WebView for Agents ###
    private void SpawnAgents()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        Agent[] agents = screenViewModel.agents;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;
        Vector3Int worldSpawnPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition) + new Vector3Int(0, 1, 0);
        foreach (Agent agent in agents)
        {
            InstantiatedAgents.Add(Instantiate(agentPrefab, worldSpawnPosition, Quaternion.identity));
        }
    }

    private void MoveAgents()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        float step = animationSpeed * Time.deltaTime;
        int index = 0;
        foreach (GameObject agent in InstantiatedAgents)
        {
            Vector2Int agentPosition = screenViewModel.agents[index].position;
            Vector3Int newWorldPosition = ConvertVector2DTo3D(agentPosition) + new Vector3Int(0, 1, 0);
            //agent.transform.position = Vector3.MoveTowards(agent.transform.position, newWorldPosition, step);
            agent.transform.position = newWorldPosition;
            index++;
        }
    }

    public void ShowOrHideAgents(bool isOn)
    {
        foreach (GameObject agent in InstantiatedAgents)
        {
            MeshRenderer meshRenderer = agent.GetComponent<MeshRenderer>();
            meshRenderer.enabled = isOn;
        }
    }

    public void ShowOrHideExploredArea(bool isOn)
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        MapObject[,] map = screenViewModel.map;
        HashSet<Point> visibleTiles = screenViewModel.visibleTiles;
        if (isOn)
        {
            foreach (MapObject mapObject in map)
            {
                Point point = new(mapObject.ArrayPosition.x, mapObject.ArrayPosition.y);
                if (!visibleTiles.Contains(point) && mapObject.Type == MapObject.ObjectType.Tile)
                {
                    int x = mapObject.ArrayPosition.x;
                    int y = mapObject.ArrayPosition.y;
                    Renderer renderer = InstantiatedMap[x, y].GetComponent<Renderer>();
                    renderer.material = darkColor;
                }
            }
        }
        else
        {
            foreach (MapObject mapObject in map)
            {
                if (mapObject.Type == MapObject.ObjectType.Tile)
                {
                    Renderer renderer = InstantiatedMap[mapObject.ArrayPosition.x, mapObject.ArrayPosition.y].GetComponent<Renderer>();
                    renderer.material = whiteColor;
                }
            }
        }
    }

    public void ShowOrHideFrontierPoints(bool isOn)
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        MapObject[,] map = screenViewModel.map;
        List<Point> frontierPoints = screenViewModel.frontierPointsForRendering;
        if (isOn)
        {
            foreach (MapObject mapObject in map)
            {
                Point point = new(mapObject.ArrayPosition.x, mapObject.ArrayPosition.y);
                if (!frontierPoints.Contains(point) && mapObject.Type == MapObject.ObjectType.Tile)
                {
                    int x = mapObject.ArrayPosition.x;
                    int y = mapObject.ArrayPosition.y;
                    Renderer renderer = InstantiatedMap[x, y].GetComponent<Renderer>();
                    renderer.material = darkColor;
                }
            }
        }
        else
        {
            foreach (MapObject mapObject in map)
            {
                if (mapObject.Type == MapObject.ObjectType.Tile)
                {
                    Renderer renderer = InstantiatedMap[mapObject.ArrayPosition.x, mapObject.ArrayPosition.y].GetComponent<Renderer>();
                    renderer.material = whiteColor;
                }
            }
        }
    }

    public void ClearFrontiers()
    {
        foreach (GameObject frontier in InstantiatedFrontiers)
        {
            if (frontier != null) Destroy(frontier);
        }
        InstantiatedFrontiers = new();
    }
    public void RenderFrontiers()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        List<Point> centroids = screenViewModel.centroidsForRendering;
        ClearFrontiers();
        foreach (Point centroid in centroids)
        {
            Vector3 frontierPosition = new(centroid.x, 1, centroid.y);
            InstantiatedFrontiers.Add(Instantiate(frontierPrefab, frontierPosition, Quaternion.identity));
        }
    }
    public void ShowOrHideFrontiers(bool isOn)
    {
        foreach (GameObject frontier in InstantiatedFrontiers)
        {
            frontier.SetActive(isOn);
        }
    }

    // ##### WebView for MMAS #####
    void RenderMMASGraph()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        if (screenViewModel.checkPoints.Count < 3) return;
        MMAS mmas = myGameManager.mmasGraphController;
        Graph graph = mmas._graph;
        ClearMMASGraph();


        // Create node objects
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            Node node = graph.Nodes[i];
            if (InstantiatedMMASNodes != null) // Only create if it doesn't already exist
            {
                InstantiatedMMASNodes.Add(Instantiate(nodePrefab, new Vector3((float)node.X, 1, (float)node.Y), Quaternion.identity));
                InstantiatedMMASNodes[i].name = "Node " + node.Id;
            }
        }

        // Create edges
        int edgeIndex = 0;
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            Node nodei = graph.Nodes[i];
            for (int j = 0; j < graph.Nodes.Count; j++)
            {
                Node nodej = graph.Nodes[j];
                if (graph.getEdge(nodei, nodej) < double.MaxValue)
                {
                    if (InstantiatedMMASEdges != null && InstantiatedMMASNodes.Count > 3) // only render if there are more than 3 
                    {
                        GameObject edge = new GameObject("Edge_" + i + "_" + j);
                        LineRenderer lr = edge.AddComponent<LineRenderer>();
                        lr.material = edgeMaterial;
                        lr.SetPositions(new Vector3[] { InstantiatedMMASNodes[i].transform.position, InstantiatedMMASNodes[j].transform.position });
                        lr.startWidth = 0.05f * 8;
                        lr.endWidth = 0.05f * 8;
                        InstantiatedMMASEdges.Add(edge);
                        UpdateEdgeTransparency(lr, mmas.getPheromone(nodei, nodej));
                    }
                    edgeIndex++;
                }
            }
        }
    }

    private void ClearMMASGraph()
    {
        foreach (GameObject node in InstantiatedMMASNodes)
        {
            Destroy(node);
        }
        foreach (GameObject edge in InstantiatedMMASEdges)
        {
            Destroy(edge);
        }
    }
    public void ShowOrHideMMASGraph(bool isOn)
    {
        foreach (GameObject node in InstantiatedMMASNodes)
            if (node != null) node.SetActive(isOn);
        foreach (GameObject edge in InstantiatedMMASEdges)
        {
            if (edge != null) edge.SetActive(isOn);
        }
    }
    void UpdateEdgeTransparency(LineRenderer edge, double pheromoneLevel)
    {
        MMAS mmas = myGameManager.mmasGraphController;
        Color color = edge.material.color;
        color.a = mmas._tauMax > 0 ? (float)(pheromoneLevel / mmas._tauMax) : 0;
        edge.material.color = color;
    }

    void HighlightBestTour()
    {
        MMAS mmas = myGameManager.mmasGraphController;
        Node[] bestTour = mmas.GetBestTour();
        for (int i = 0; i < bestTour.Length - 1; i++)
        {
            Node startIndex = bestTour[i];
            Node endIndex = bestTour[i + 1];
            GameObject edge = InstantiatedMMASEdges[startIndex.Id * mmas._graph.Nodes.Count + endIndex.Id];
            LineRenderer lr = edge.GetComponent<LineRenderer>();
            lr.material = tourMaterial;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
        }
    }
}


