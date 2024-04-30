using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GraphPresenter : MonoBehaviour
{
    public float tileSize = 1.0f;
    public float nodeScale = 0.1f;
    public bool showClusters = true;
    public bool showIntraEdges = true;
    public bool showInterEdges = true;

    public bool showNodes = true;

    public bool showEntrances = true;
    public GameObject nodePrefab;

    public GameObject entrancePrefab;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;

    public Material clusterMaterial;

    public int visualizeLevel = 1;

    private IGraphModel _graphModel;
    private IPathFinder _pathFinder;
    private INodeManager _nodeManager;

    void Start()
    {
        Debug.Log("Start graph visualizer");
        int maxSize = 100;
        int[,] tileMap = new int[maxSize, maxSize];
        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                tileMap[i, j] = 0;
            }
        }




        // Create instances of the necessary classes
        _pathFinder = new PathFinder();
        _graphModel = new GraphModel(tileMap);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(visualizeLevel);

        // for (int i = 0; i < maxSize / 3; i++)
        // {
        //     for (int j = 0; j < maxSize / 3; j++)
        //     {
        //         tileMap[i, j] = 0;
        //         HPAStar.DynamicallyAddHPANode(new Vector2Int(i, j), false);
        //     }
        // }

        // // finalize cluster 
        // foreach (Cluster cluster in _graphModel.ClusterByLevel[1])
        // {
        //     cluster.isFinalized = true;
        // }
        // Cluster first = _graphModel.ClusterByLevel[1].First();
        // first.isFinalized = true;
        // HPAStar.DynamicallyAddHPANode(first.Nodes.First().Position, true);


        for (int i = 5; i < maxSize / 4; i++)
        {
            for (int j = 5; j < maxSize / 4; j++)
            {
                tileMap[i, j] = 1;
                HPAStar.DynamicallyRemoveHPANode(new Vector2Int(i, j));
            }
        }





        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(maxSize - 1, maxSize - 1);

        // List<HPANode> nodes = HPAStar.HierarchicalSearch(start, goal, 1);
        // List<HPANode> nodes = HPAStar.HierarchicalSearch(new Vector2Int(15, 11), new Vector2Int(29, 4), 1);

        // foreach (HPANode node in nodes)
        // {
        //     Debug.Log(node.Position);
        // }
        DrawGraph();
    }

    void DrawGraph()
    {
        if (_graphModel.ClusterByLevel.TryGetValue(visualizeLevel, out var clusters))
        {
            if (showClusters)
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






                    if (showEntrances)
                        drawEntrances(cluster);
                    foreach (var node in cluster.Nodes)
                    {

                        if (showNodes)
                            DrawNode(node);
                        if (showIntraEdges)
                            DrawEdges(node, HPAEdgeType.INTRA);
                        if (showInterEdges)
                            DrawEdges(node, HPAEdgeType.INTER);
                    }
                }
            }
        }
        else
        {
            Debug.Log($"No data available for level {visualizeLevel}");
        }
    }

    void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        nodeObj.SetActive(true);
    }

    void drawEntrances(Cluster cluster)
    {
        foreach (Entrance entrance in cluster.Entrances)
        {
            GameObject entranceObj = Instantiate(entrancePrefab, transform.position + new Vector3(entrance.Node1.Position.x * tileSize, 0, entrance.Node1.Position.y * tileSize), Quaternion.identity);
            entranceObj.transform.localScale = Vector3.one * nodeScale * tileSize;
            entranceObj.SetActive(true);




        }
    }

    void DrawEdges(HPANode node, HPAEdgeType edgeType)
    {
        foreach (HPAEdge edge in node.Edges)
        {
            if (edge.Type != edgeType) continue;
            Vector3 start = transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(edge.Node2.Position.x * tileSize, 0, edge.Node2.Position.y * tileSize);
            DrawLine(start, end, edgeType == HPAEdgeType.INTRA ? intraEdgeMaterial : interEdgeMaterial);
        }
    }



    void DrawLine(Vector3 start, Vector3 end, Material material)
    {
        GameObject lineObj = new GameObject("Edge");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = material;
        lr.startWidth = 0.05f * tileSize;
        lr.endWidth = 0.05f * tileSize;
        lr.SetPositions(new Vector3[] { start, end });
    }
}