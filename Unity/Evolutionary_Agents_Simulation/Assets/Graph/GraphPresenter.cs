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

    public bool visualizePath = true;

    private IGraphModel _graphModel;
    private IPathFinder _pathFinder;
    private INodeManager _nodeManager;

    void Start()
    {
        


        // Cluster c1 = clusterManager.DetermineCluster(new Vector2Int(9, 5), visualizeLevel);
        // Debug.Log("total entrances in cluster" + c1.Entrances.Count);
        // c1.isFinalized = true;
        // tileMap[9, 5] = 1;
        // HPAStar.DynamicallyRemoveHPANode(new Vector2Int(9, 5));

        // Debug.Log("total entrances in cluster" + c1.Entrances.Count);

        // HPAPath hpaPath = HPAStar.HierarchicalSearch(new Vector2Int(0, 0), new Vector2Int(99, 99), 1);

        // Debug.Log("Path Length: " + hpaPath.Length);
        // Debug.Log("Nodes Explored: " + hpaPath.NodesExplored);



        // Cluster c = _graphModel.ClusterByLevel[1].First();

        // foreach (Cluster cluster in _graphModel.ClusterByLevel[1])
        // {
        //     foreach (Entrance entrance in c.Entrances)
        //     {
        //         Debug.Log("total entrances" + c.Entrances.Count);
        //         Debug.Log(entrance.Node1.Position);
        //         Debug.Log(entrance.Node2.Position);
        //     }
        // }




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


        // for (int i = 5; i < maxSize / 4; i++)
        // {
        //     for (int j = 5; j < maxSize / 4; j++)
        //     {
        //         tileMap[i, j] = 1;
        //         HPAStar.DynamicallyRemoveHPANode(new Vector2Int(i, j));
        //     }
        // }





       /* Vector2Int goal = new Vector2Int(0, 0);
        Vector2Int start = new Vector2Int(maxSize - 3, maxSize - 3);

        // HPAPath nodes = HPAStar.HierarchicalSearch(start, goal, 1);
        // HPAPath nodes2 = HPAStar.HierarchicalAbstractSearch(start, goal, visualizeLevel);
        Cluster c4 = clusterManager.DetermineCluster(goal, visualizeLevel);

        // _nodeManager.FindOrCreateNode(goal.x, goal.y, c4);

        // _nodeManager.insertCheckpoint(start, visualizeLevel);
        // _nodeManager.insertCheckpoint(goal, visualizeLevel);

        HPAPath path = HPAStar.HierarchicalAbstractSearch(start, goal, visualizeLevel);
        Debug.Log("Path Length: " + path.Length);
        Debug.Log("Nodes Explored: " + path.NodesExplored);

        HPAPath path2 = HPAStar.HierarchicalSearch(start, goal, visualizeLevel);
        Debug.Log("Path Length: " + path2.Length);
        Debug.Log("Nodes Explored: " + path2.NodesExplored);*/




        // Debug.Log("Path Length: " + nodes2.path.Count);
        // List<HPANode> nodes = HPAStar.HierarchicalSearch(new Vector2Int(15, 11), new Vector2Int(29, 4), 1);

        // foreach (HPANode node in nodes)
        // {
        //     Debug.Log(node.Position);
        // }
        /*if (visualizePath)
        {
            drawPath(path);
        }

        if (visualizePath)
        {
            drawPath(path2);
        }
        DrawGraph(visualizeLevel);*/
    }

    void DrawGraph(int level)
    {
        if (_graphModel.ClusterByLevel.TryGetValue(level, out var clusters))
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
            Debug.Log($"No data available for level {level}");
        }
    }

    void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        nodeObj.SetActive(true);
    }

    void drawPath(HPAPath path)
    {
        for (int i = 0; i < path.path.Count - 1; i++)
        {
            Vector3 start = transform.position + new Vector3(path.path[i].Position.x * tileSize, 0, path.path[i].Position.y * tileSize);
            Vector3 end = transform.position + new Vector3(path.path[i + 1].Position.x * tileSize, 0, path.path[i + 1].Position.y * tileSize);
            DrawLine(start, end, intraEdgeMaterial);
        }
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