using UnityEngine;
using System.Collections.Generic;

public class GraphPresenter : MonoBehaviour
{
    public float tileSize = 1.0f;
    public float nodeScale = 0.1f;
    public bool showClusters = true;
    public bool showIntraEdges = true;
    public bool showInterEdges = true;
    public GameObject nodePrefab;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;

    private IGraphModel _graphModel;
    private IPathFinder _pathFinder;
    private INodeManager _nodeManager;

    void Start()
    {
        Debug.Log("Start graph visualizer");
        int maxSize = 30;
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
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IHPAStar HPAStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);

        HPAStar.Preprocessing(1);

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int goal = new Vector2Int(maxSize - 1, maxSize - 1);

        // List<HPANode> nodes = HPAStar.HierarchicalSearch(start, goal, 1);
        List<HPANode> nodes = HPAStar.HierarchicalSearch(new Vector2Int(15, 11), new Vector2Int(29, 4), 1);

        foreach (HPANode node in nodes)
        {
            Debug.Log(node.Position);
        }
        DrawGraph();
    }

    void DrawGraph()
    {
        foreach (var level in _graphModel.ClusterByLevel)
        {
            if (showClusters)
            {
                foreach (var cluster in level.Value)
                {
                    foreach (var node in cluster.Nodes)
                    {
                        DrawNode(node);
                        if (showIntraEdges)
                            DrawEdges(node, HPAEdgeType.INTRA);
                        if (showInterEdges)
                            DrawEdges(node, HPAEdgeType.INTER);
                    }
                }
            }
        }
    }

    void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        nodeObj.SetActive(true);
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