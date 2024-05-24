using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;
using UnityEngine.UI;

public class WebView : MonoBehaviour, IScreenView
{

    // Game Objects prefabs
    public GameObject wallPrefab;
    public GameObject tilePrefab;
    public GameObject checkPointPrefab;
    public GameObject spawnPointPrefab;
    public GameObject agentPrefab;
    public GameObject nodePrefab;
    public GameObject entrancePrefab;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;
    public Material clusterMaterial;

    // Local variables
    private GameObject[,] InstantiatedMap;
    private List<GameObject> InstantiatedGraph;
    private readonly float tileScale = 0.1f;
    private readonly int tileSize = 1;
    private readonly float nodeScale = 0.1f;

    // Presenter
    private ScreenPresenter screenPresenter;
    
    void Start()
    {
        MyGameManager myGameManager = new();
        screenPresenter = new(myGameManager);
        RenderMap(screenPresenter.PackageData());
        //RenderGraph(1,screenPresenter.PackageData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RenderMap(ScreenViewModel screenViewModel){
        MapObject[,] map = screenViewModel.map;
        List<CheckPoint> checkPoints = screenViewModel.checkPoints;
        AgentSpawnPoint spawnPoint = screenViewModel.spawnPoint;

        ClearMap(InstantiatedMap);
        InstantiatedMap = new GameObject[map.GetLength(1),map.GetLength(0)];
        InstantiateMap(map); 
        InstantiateCheckPoints(checkPoints);
        InstantiateSpawnPoint(spawnPoint);

    }
    private void InstantiateMap(MapObject[,] map){
        int mapWidth = (int) (map.GetLength(0) * tileScale);
        int mapHeight = (int) (map.GetLength(1) * tileScale);
        Vector3Int position = new Vector3Int(map.GetLength(0)/2, 0, map.GetLength(1)/2);
        GameObject floor = Instantiate(tilePrefab, position, Quaternion.identity);
        floor.transform.position = position;
        floor.transform.localScale = new Vector3(mapWidth,0,mapHeight);

        foreach(MapObject mapObject in map){
            Vector3Int worldPosition = ConvertVector2DTo3D(mapObject.ArrayPosition);
            int i = mapObject.ArrayPosition.x;
            int j = mapObject.ArrayPosition.y;
            if(mapObject.Type == MapObject.ObjectType.Wall){
                InstantiatedMap[i,j] = Instantiate(wallPrefab,worldPosition,Quaternion.identity);
            }
        }
    }

    private void InstantiateCheckPoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            Vector3Int worldPosition = ConvertVector2DTo3D(checkPoint.ArrayPosition);
            int i = checkPoint.ArrayPosition.x;
            int j = checkPoint.ArrayPosition.y;
            InstantiatedMap[i,j] = Instantiate(checkPointPrefab,worldPosition,Quaternion.identity);
        } 
    }
    private void InstantiateSpawnPoint(AgentSpawnPoint spawnPoint){
        Vector3Int worldPosition = ConvertVector2DTo3D(spawnPoint.ArrayPosition);
        int i = spawnPoint.ArrayPosition.x;
        int j = spawnPoint.ArrayPosition.y;
        InstantiatedMap[i,j] = Instantiate(spawnPointPrefab,worldPosition,Quaternion.identity);
    }   
    private Vector3Int ConvertVector2DTo3D(Vector2Int arrayPosition){
        return new Vector3Int(arrayPosition.x,0,arrayPosition.y);
    }

    private void ClearMap(GameObject[,] InstantiatedMap){
        if(InstantiatedMap == null) return;
        foreach(GameObject mapObject in InstantiatedMap){
            Destroy(mapObject);
        }
    }

    // Graph begins
    private void RenderGraph(int level, ScreenViewModel screenViewModel) {
        ClearGraph(InstantiatedGraph);
        InstantiatedGraph = new();
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

    private void ClearGraph(List<GameObject> InstantiatedGraph){
        if(InstantiatedGraph == null) return;
        foreach(GameObject graphObject in InstantiatedGraph){
            Destroy(graphObject);
        }
    }

    private void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        nodeObj.SetActive(true);
        InstantiatedGraph.Add(nodeObj);
    }

    private void DrawPath(HPAPath path)
    {
        for (int i = 0; i < path.path.Count - 1; i++)
        {
            Vector3 start = transform.position + new Vector3(path.path[i].Position.x * tileSize, 0, path.path[i].Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(path.path[i + 1].Position.x * tileSize, 0, path.path[i + 1].Position.y * tileSize);
            DrawLine(start, end, intraEdgeMaterial);
        }
    }

    private void DrawEntrances(Cluster cluster)
    {
        foreach (Entrance entrance in cluster.Entrances)
        {
            GameObject entranceObj = Instantiate(entrancePrefab, transform.position + new Vector3(entrance.Node1.Position.x * tileSize, 0, entrance.Node1.Position.y * tileSize), Quaternion.identity);
            entranceObj.transform.localScale = Vector3.one * nodeScale * tileSize;
            entranceObj.SetActive(true);
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
        GameObject lineObj = new GameObject("Edge");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = 0.05f * tileSize;
        lr.endWidth = 0.05f * tileSize;
        lr.SetPositions(new Vector3[] { start, end });
        InstantiatedGraph.Add(lineObj);
    }

}
