using System;
using UnityEngine;

public class MyGameManager
{

    // Object for Custom Maps
    CustomMaps customMaps;


    // Controllers
    public MapController mapController;
    public HPAStar HPAGraphController;
    public MMAS mmasGraphController;
    public AgentController agentController;


    // Main
    public void main()
    {

        mapController.ChangeMapParameters(customMaps.GetCustomMap(1));

    }


    //Run backend here

    public MyGameManager()
    {
        customMaps = new();
        MapModel mapModel = customMaps.GetCustomMap(0);
        mapController = new MapController(mapModel);
        AgentModel agentModel = new(1, mapModel.map, mapModel.spawnPoint);
        agentController = new AgentController(agentModel);
        HPAGraphController = InitialiseHPAStar(mapModel.map);
        mmasGraphController = InitialiseMMMAS();
        agentController.Scan();

    }

    public MyGameManager(MapModel mapModel)
    {
        mapController = new MapController(mapModel);
        AgentModel agentModel = new(1, mapModel.map, mapModel.spawnPoint);
        agentController = new AgentController(agentModel);
        HPAGraphController = InitialiseHPAStar(mapModel.map);
        mmasGraphController = InitialiseMMMAS();
    }


    private HPAStar InitialiseHPAStar(MapObject[,] map)
    {
        GraphModel _graphModel = new GraphModel(map);
        PathFinder _pathFinder = new PathFinder(new GraphModel(map));
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        HPAStar hpaStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);
        int maxLevel = HPAStar.maxLevelAllowed(map.GetLength(0), clusterSize: 10);
        hpaStar.Preprocessing(maxLevel);
        return hpaStar;
    }

    private MMAS InitialiseMMMAS()
    {
        Graph graph = new Graph();
        int numAnts = 0;
        double alpha = 1.5;
        double beta = 4.5;
        double rho = 0.90;
        double q = 100.0;
        // int maxIterations = 500;

        MMAS mmas = new MMAS(numAnts, alpha, beta, rho, q, graph);
        // mmas.SetGraph(graph);
        // mmas.Run(maxIterations);
        return mmas;
    }

    public void MmasAddCheckpoint(Vector2Int checkpoint, int heuristicsLevel, int iterations = 0, bool linearHeuristic = true)
    {
        if (mmasGraphController._graph == null)
        {
            Debug.LogError("Graph is null");
        }


        Node newNode = new Node(mmasGraphController._graph.Nodes.Count, checkpoint.x, checkpoint.y);

        if (mmasGraphController._graph.Nodes.Count < 3)
        {
            mmasGraphController._graph.AddNode(newNode); // adds nodes statically
            CalculateEdges(heuristicsLevel, linearHeuristic, newNode);
        }
        else if (mmasGraphController._graph.Nodes.Count == 3) // ensures graph is built when 3 checkpoints are added.
        {
            mmasGraphController._graph.AddNode(newNode);

            CalculateEdges(heuristicsLevel, linearHeuristic, newNode);
            mmasGraphController.SetGraph(mmasGraphController._graph);
        }
        else
        {
            mmasGraphController.AddNode(newNode); // adds Dynamiclly.  
            CalculateEdges(heuristicsLevel, linearHeuristic, newNode);

        }
        mmasGraphController._numAnts = mmasGraphController._graph.Nodes.Count;

        if (iterations > 0 && mmasGraphController._graph.Nodes.Count > 3)
        {
            mmasGraphController.Run(iterations);
        }

    }

    private void CalculateEdges(int heuristicsLevel, bool linearHeuristic, Node newNode)
    {
        foreach (Node node in mmasGraphController._graph.Nodes)
        {
            if (node != newNode)
            {
                Vector2Int nodePosition = new Vector2Int((int)node.X, (int)node.Y);
                Vector2Int newNodePosition = new Vector2Int((int)newNode.X, (int)newNode.Y);
                double distance = double.PositiveInfinity;

                if (linearHeuristic)
                {
                    distance = Vector2Int.Distance(nodePosition, newNodePosition);
                }
                else
                {
                    HPAPath path = HPAGraphController.HierarchicalAbstractSearch(nodePosition, newNodePosition, heuristicsLevel);

                    if (path != null)
                    {

                        distance = path.Length;
                    }
                }

                mmasGraphController._graph.AddEdge(node, newNode, distance);
                mmasGraphController._graph.AddEdge(newNode, node, distance);
            }
        }
    }

    public void MmasRemoveCheckpoint(Vector2Int checkpoint, int iterations = 0)
    {


        Node nodeToRemove = mmasGraphController._graph.Nodes.Find(x => x.X == checkpoint.x && x.Y == checkpoint.y); // TODO: can be optimized. 
        if (nodeToRemove == null)
        {
            return;
        }
        mmasGraphController.RemoveNode(nodeToRemove);
        mmasGraphController._numAnts = mmasGraphController._graph.Nodes.Count;

        if (iterations > 0)
        {
            mmasGraphController.Run(iterations);
        }
    }



}

