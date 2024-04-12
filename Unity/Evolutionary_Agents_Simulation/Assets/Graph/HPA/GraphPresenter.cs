using UnityEngine;
using System.Collections.Generic;

public class GraphPresenter : MonoBehaviour
{
    public float tileSize = 1.0f;  // Define your tile size
    public float nodeScale = 0.1f;
    public bool showClusters = true;
    public bool showIntraEdges = true;
    public bool showInterEdges = true;

    public GameObject nodePrefab;
    public Material intraEdgeMaterial;
    public Material interEdgeMaterial;

    private HPAStarGraphConstruction graph; // Your graph construction class



    void Start()
    {
        Debug.Log("Start graph visualizer");


        int maxSize = 30;

        int[,] tileMap = new int[maxSize, maxSize]; // Define your tile map

        for (int i = 0; i < maxSize; i++)
        {
            for (int j = 0; j < maxSize; j++)
            {
                // wall or tile randomly 
                // if (Random.Range(0, 100) < 30)
                //     tileMap[i, j] = 1;
                // else
                tileMap[i, j] = 0;
            }
        }
        graph = new HPAStarGraphConstruction(tileMap); // Make sure your graph is already created and populated





        graph.Preprocessing(1); // Specify the maximum level for preprocessing

        Vector2Int start = new Vector2Int(0, 0);
        Vector2Int end = new Vector2Int(maxSize - 1, maxSize - 1);

        HPANode startNode = new HPANode(1251251251, null, start, 1);
        HPANode endNode = new HPANode(1251251253, null, end, 1);

        graph.insertNode(startNode, 1);
        graph.insertNode(endNode, 1);

        List<HPANode> abstractPath = graph.searchForPath(start, end, 1);

        if (abstractPath != null)
        {
            Debug.Log(" abstract path found");

        }
        else
        {
            Debug.Log("No path found");
        }

        // Refine the abstract path
        List<HPANode> detailedPath = graph.refinePath(abstractPath, 1);

        if (detailedPath != null)
        {
            foreach (HPANode node in detailedPath)
            {

                Debug.Log("refined path: " + node.Position);
            }
        }
        else
        {
            Debug.Log("No detailed path found");
        }






        DrawGraph();
    }





    void DrawGraph()
    {
        foreach (var level in graph.ClusterByLevel)
        {
            if (showClusters)
                // DrawClusters(level.Value);

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



    void DrawNode(HPANode node)
    {
        GameObject nodeObj = Instantiate(nodePrefab, transform.position + new Vector3(node.Position.x * tileSize, 0, node.Position.y * tileSize), Quaternion.identity);
        nodeObj.transform.localScale = Vector3.one * nodeScale * tileSize;
        nodeObj.SetActive(true); // Reactivate for each instance
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
        lr.startWidth = 0.05f * tileSize;  // Adjust line width based on tile size
        lr.endWidth = 0.05f * tileSize;
        lr.SetPositions(new Vector3[] { start, end });
    }
}
