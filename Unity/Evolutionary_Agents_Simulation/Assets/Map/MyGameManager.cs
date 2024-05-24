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
        MapModel mapModel = customMaps.GetCustomMap(8);
        mapController = new MapController(mapModel);
        agentController = new AgentController(new AgentModel(1));
        HPAGraphController = InitialiseHPAStar(mapModel.map);
        mmasGraphController = InitialiseMMMAS();

    }

    private HPAStar InitialiseHPAStar(MapObject[,] map)
    {
        GraphModel _graphModel = new GraphModel(map);
        PathFinder _pathFinder = new PathFinder(_graphModel);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        HPAStar hpaStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);
        hpaStar.Preprocessing(3);
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

        MMAS mmas = new MMAS(numAnts, alpha, beta, rho, q);
        // mmas.SetGraph(graph);
        // mmas.Run(maxIterations);
        return mmas;

    }

}

