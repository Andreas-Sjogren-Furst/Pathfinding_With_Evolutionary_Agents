using UnityEngine;

public class MMASVisualizer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject nodePrefab; // Assign a prefab for the nodes
    public Material edgeMaterial; // Assign a material for the edges
    public Material tourMaterial; // Assign a material for highlighting the to

    public bool showRemoval = false;

    public bool showAddition = false;

    public int checkpoints;

    private Graph graph;
    private MMAS mmas;

    private GameObject[] nodeObjects;
    private LineRenderer[] edgeRenderers;

    private Node[] nodes;

    void Start()
    {

        // graph = new Graph();

        // // init graph: 

        // nodes = new Node[checkpoints];
        // for (int i = 0; i < checkpoints; i++)
        // {
        //     System.Random random = new System.Random();

        //     int x = random.Next(0, 100);
        //     int y = random.Next(0, 100);
        //     nodes[i] = new Node(i, x, y);
        //     graph.AddNode(nodes[i]);
        // }

        // for (int i = 0; i < checkpoints; i++)
        // {
        //     for (int j = i + 1; j < checkpoints; j++)
        //     {
        //         double distance = Vector2Int.Distance(new Vector2Int((int)nodes[i].X, (int)nodes[i].Y), new Vector2Int((int)nodes[j].X, (int)nodes[j].Y));
        //         graph.AddEdge(nodes[i], nodes[j], distance);
        //         graph.AddEdge(nodes[j], nodes[i], distance);
        //     }
        // }



        // int numAnts = checkpoints;
        // double alpha = 1.5;
        // double beta = 4.5;
        // double rho = 0.90;
        // double q = 100.0;
        // int maxIterations = 500;

        // mmas = new MMAS(numAnts, alpha, beta, rho, q, graph);
        // mmas.SetGraph(graph);
        // mmas.Run(maxIterations);
        // Node[] bestTour = mmas.GetBestTour();
        // double bestTourLength = mmas.GetBestTourLength();

        // // Debug.Log("Best Tour: " + string.Join(" -> ", bestTour));

        // InitializeVisualization(graph, mmas);



    }

    // Update is called once per frame
    void Update()
    {
        // if (showRemoval)
        // {
        //     showRemoval = false;

        //     mmas.RemoveNode(nodes.Last());
        //     nodes = nodes.Take(nodes.Length - 1).ToArray();

        //     mmas._numAnts = mmas._graph.Nodes.Count;
        //     Debug.Log("Num Ants: " + mmas._numAnts);
        //     Debug.Log("Num Nodes: " + mmas._graph.Nodes.Count);
        //     mmas.Run(1);
        //     RemovedGameObjectAndLines();
        //     InitializeVisualization(mmas._graph, mmas);
        // }

        // if (showAddition)
        // {
        //     showAddition = false;

        //     System.Random random = new System.Random();

        //     int x = random.Next(0, 100);
        //     int y = random.Next(0, 100);
        //     Node newNode = new Node(nodes.Length, x, y);
        //     nodes = nodes.Concat(new Node[] { newNode }).ToArray();
        //     mmas.AddNode(newNode);
        //     mmas._numAnts = mmas._graph.Nodes.Count;

        //     foreach (Node node in mmas._graph.Nodes)
        //     {
        //         if (node != newNode)
        //         {
        //             double distance = Vector2Int.Distance(new Vector2Int((int)node.X, (int)node.Y), new Vector2Int((int)newNode.X, (int)newNode.Y));
        //             mmas._graph.AddEdge(node, newNode, distance);
        //             mmas._graph.AddEdge(newNode, node, distance);
        //         }
        //     }
        //     Debug.Log("Num Ants: " + mmas._numAnts);
        //     Debug.Log("Num Nodes: " + mmas._graph.Nodes.Count);
        //     mmas.Run(100);
        //     RemovedGameObjectAndLines();
        //     InitializeVisualization(mmas._graph, mmas);
        // }
    }

    public void RemovedGameObjectAndLines()
    {
        // Ensure nodeObjects is initialized and contains items
        if (nodeObjects != null)
        {
            for (int i = 0; i < nodeObjects.Length; i++)
            {
                if (nodeObjects[i] != null)
                    Destroy(nodeObjects[i]);
            }
            nodeObjects = null; // Clear the array after destroying objects
        }

        // Ensure edgeRenderers is initialized and contains items
        if (edgeRenderers != null)
        {
            for (int i = 0; i < edgeRenderers.Length; i++)
            {
                if (edgeRenderers[i] != null)
                    Destroy(edgeRenderers[i].gameObject);
            }
            edgeRenderers = null; // Clear the array after destroying objects
        }
    }




    public void InitializeVisualization(Graph graph, MMAS mmas)
    {
        Debug.Log("Initializing visualization for " + graph.Nodes.Count + " nodes.");

        // Clean up old visualization if exists
        RemovedGameObjectAndLines();

        // Reinitialize arrays for node objects and edge renderers
        nodeObjects = new GameObject[graph.Nodes.Count];
        edgeRenderers = new LineRenderer[graph.Nodes.Count * graph.Nodes.Count]; // This may need adjustment based on actual edge count

        DrawGraph(graph, mmas);
        // HighlightBestTour(mmas);
    }


    void DrawGraph(Graph graph, MMAS mmas)
    {
        // Create node objects
        for (int i = 0; i < graph.Nodes.Count; i++)
        {
            Node node = graph.Nodes[i];
            if (nodeObjects[i] == null) // Only create if it doesn't already exist
            {
                nodeObjects[i] = Instantiate(nodePrefab, new Vector3((float)node.X, 1, (float)node.Y), Quaternion.identity);
                nodeObjects[i].name = "Node " + node.Id;
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
                    if (edgeRenderers[edgeIndex] == null) // Only create if it doesn't already exist
                    {
                        LineRenderer lr = new GameObject("Edge_" + i + "_" + j).AddComponent<LineRenderer>();
                        lr.material = edgeMaterial;
                        lr.SetPositions(new Vector3[] { nodeObjects[i].transform.position, nodeObjects[j].transform.position });
                        lr.startWidth = 0.05f;
                        lr.endWidth = 0.05f;
                        edgeRenderers[edgeIndex] = lr;
                        UpdateEdgeTransparency(lr, mmas.getPheromone(nodei, nodej));
                    }
                    edgeIndex++;
                }
            }
        }
    }


    void UpdateEdgeTransparency(LineRenderer edge, double pheromoneLevel)
    {
        Color color = edge.material.color;
        color.a = mmas._tauMax > 0 ? (float)(pheromoneLevel / mmas._tauMax) : 0;
        edge.material.color = color;
    }

    void HighlightBestTour()
    {
        Node[] bestTour = mmas.GetBestTour();
        for (int i = 0; i < bestTour.Length - 1; i++)
        {
            Node startIndex = bestTour[i];
            Node endIndex = bestTour[i + 1];
            LineRenderer lr = edgeRenderers[startIndex.Id * graph.Nodes.Count + endIndex.Id];
            lr.material = tourMaterial;
            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
        }
    }
}
