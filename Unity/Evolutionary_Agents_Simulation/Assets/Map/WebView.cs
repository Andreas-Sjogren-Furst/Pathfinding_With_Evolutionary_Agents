using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.UIElements;

public class WebView : MonoBehaviour, IScreenView
{

    // MMAS materials
    public Material edgeMaterial; // Assign a material for the edges
    public Material tourMaterial; // Assign a material for highlighting the to

    public static WebView Instance { get; private set; }
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
    private string nodeTag = "Node";
    private string entranceTag = "Entrance";
    private string lineTag = "Line";



    // Local variables
    private List<GameObject> InstantiatedWalls;
    private List<GameObject> InstantiatedCheckPoints;
    private GameObject InstantiatedSpawnPoint;
    private GameObject InstantiatedFloor;
    private List<GameObject> InstantiatedGraph;
    private List<GameObject> InstantiatedAgents;
    private GameObject[] InstantiatedNodes;
    private LineRenderer[] InstantiatedEdges;
    private readonly float tileScale = 0.1f;
    private readonly int tileSize = 1;
    private readonly float nodeScale = 0.1f;
    private readonly float animationSpeed = 5f;


    // Presenter
    public ScreenPresenter screenPresenter;

    // GameManager
    private MyGameManager myGameManager;
    
    void Awake()
    {
        Instance = this;
        myGameManager = new();
        screenPresenter = new(myGameManager);
        InstantiatedCheckPoints = new();
        InstantiatedWalls = new();

        ScreenViewModel screenViewModel = screenPresenter.PackageData();

        int mapSize = screenViewModel.map.GetLength(0) * screenViewModel.map.GetLength(1);
        InstantiatedGraph = new List<GameObject>();

        // Add pools programmatically
        objectPooler.AddPool(nodeTag, nodePrefab, mapSize);
        objectPooler.AddPool(entranceTag, entrancePrefab, mapSize / 10);
        objectPooler.AddPool(lineTag, linePrefab, mapSize * 5);
    }

    void Start()
    {
        RenderMap();
        RenderHPAGraph(1);
        SpawnAgents();
        RenderMMASGraph();

        // myGameManager.graphController.Preprocessing(3);
        //Vector2Int start = screenViewModel.checkPoints[0].ArrayPosition;
        //Vector2Int end = screenViewModel.spawnPoint.ArrayPosition;
        //HPAPath path = myGameManager.HPAGraphController.HierarchicalSearch(start, end, 2);

        //DrawPath(path);


    }

    void Update()
    {

    }
    public void CreateMap(MapModel mapModel){
        myGameManager.mapController.ChangeMapParameters(mapModel);
    }
    public void RenderMap()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        MapObject[,] map = screenViewModel.map;
        List<CheckPoint> checkPoints = screenViewModel.checkPoints;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;

        ClearMap();
        InstantiateFloor(map);
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
            if (mapObject.Type == MapObject.ObjectType.Wall)
            {
                InstantiatedWalls.Add(Instantiate(wallPrefab, worldPosition, Quaternion.identity));
            }
        }
    }
    private void InstantiateFloor(MapObject[,] map){
        int mapWidth = (int)(map.GetLength(0) * tileScale);
        int mapHeight = (int)(map.GetLength(1) * tileScale);
        Vector3Int position = new Vector3Int(map.GetLength(0) / 2, 0, map.GetLength(1) / 2);
        InstantiatedFloor = Instantiate(tilePrefab, position, Quaternion.identity);
        InstantiatedFloor.transform.position = position;
        InstantiatedFloor.transform.localScale = new Vector3(mapWidth, 0, mapHeight);
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
        if (InstantiatedSpawnPoint == null || InstantiatedFloor == null) return;
        foreach (GameObject wall in InstantiatedWalls)
            if (wall != null) Destroy(wall);
            
        foreach(GameObject checkPoint in InstantiatedCheckPoints)
            if(checkPoint != null) Destroy(checkPoint);

        Destroy(InstantiatedSpawnPoint);
        Destroy(InstantiatedFloor);
    }

    public void ShowOrHideMap(bool isOn){
        foreach (GameObject wall in InstantiatedWalls)
            if (wall != null) wall.SetActive(isOn);
            
        foreach(GameObject checkPoint in InstantiatedCheckPoints)
            if(checkPoint != null) checkPoint.SetActive(isOn);

        InstantiatedSpawnPoint.SetActive(isOn);
        InstantiatedFloor.SetActive(isOn);
    }

    // ### Webview for Graph ###
    private void RenderHPAGraph(int level)
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        ClearGraph(InstantiatedGraph);
        InstantiatedGraph = new List<GameObject>();
        IGraphModel graph = screenViewModel.hpaGraph;
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
            Destroy(graphObject);
        }
    }

    public void ShowOrHideGraph(bool isOn){
        foreach (GameObject graphObject in InstantiatedGraph)
            if (graphObject != null) graphObject.SetActive(isOn);
    }
    
    private void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab ,transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
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
            GameObject entranceObj = Instantiate(entrancePrefab, transform.position + new Vector3(entrance.Node1.Position.x * tileSize, 0, entrance.Node1.Position.y * tileSize), Quaternion.identity);
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
        GameObject lineObj = Instantiate(linePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = 0.05f * tileSize;
        lr.endWidth = 0.05f * tileSize;
        lr.SetPositions(new Vector3[] { start, end });
        InstantiatedGraph.Add(lineObj);
    }

    // ### WebView for Agents ###
    private void SpawnAgents(){
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        Agent[] agents = screenViewModel.agents;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;
        Vector3Int worldSpawnPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition) + new Vector3Int(0,1,0);
        foreach(Agent agent in agents){
            InstantiatedAgents.Add(Instantiate(agentPrefab,worldSpawnPosition,Quaternion.identity));
        }
    }

    private void MoveAgents(){
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        float step = animationSpeed * Time.deltaTime;
        int index = 0;
        foreach(GameObject agent in InstantiatedAgents){
            Vector2Int agentPosition = screenViewModel.agents[index].position;
            Vector3Int newWorldPosition = ConvertVector2DTo3D(agentPosition) + new Vector3Int(0,1,0);
            agent.transform.position = Vector3.MoveTowards(agent.transform.position, newWorldPosition, step);
            index++;
        }
    }

    public void ShowOrHideAgents(bool isOn){
        foreach(GameObject agent in InstantiatedAgents){
            agent.SetActive(isOn);
        }
    }


    // ##### WebView for MMAS #####
    void RenderMMASGraph()
    {
        ScreenViewModel screenViewModel = screenPresenter.PackageData();
        if(screenViewModel.checkPoints.Count < 3) return;
        MMAS mmas = myGameManager.mmasGraphController;
        Graph graph = mmas._graph;
        ClearMMASGraph();

        List<CheckPoint> checkPoints = screenViewModel.checkPoints;
        int index = 0;
        foreach(CheckPoint checkPoint in checkPoints){
            myGameManager.MmasAddCheckpoint(checkPoint.ArrayPosition, 1);
        }
        // Create node objects
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            Node node = graph.Nodes[i];
            if (InstantiatedNodes[i] == null) // Only create if it doesn't already exist
            {
                InstantiatedNodes[i] = Instantiate(nodePrefab, new Vector3((float)node.X, 1, (float)node.Y), Quaternion.identity);
                InstantiatedNodes[i].name = "Node " + node.Id;
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
                    if (InstantiatedEdges[edgeIndex] == null) // Only create if it doesn't already exist
                    {
                        LineRenderer lr = new GameObject("Edge_" + i + "_" + j).AddComponent<LineRenderer>();
                        lr.material = edgeMaterial;
                        lr.SetPositions(new Vector3[] { InstantiatedNodes[i].transform.position, InstantiatedNodes[j].transform.position });
                        lr.startWidth = 0.05f;
                        lr.endWidth = 0.05f;
                        InstantiatedEdges[edgeIndex] = lr;
                        UpdateEdgeTransparency(lr, mmas.getPheromone(nodei, nodej));
                    }
                    edgeIndex++;
                }
            }
        }
    }

    private void ClearMMASGraph(){
        foreach(GameObject node in InstantiatedNodes){
            Destroy(node);
        }
        foreach(LineRenderer edge in InstantiatedEdges){
            Destroy(edge);
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
            LineRenderer lr = InstantiatedEdges[startIndex.Id * mmas._graph.Nodes.Count + endIndex.Id];
            lr.material = tourMaterial;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
        }
    }
}


