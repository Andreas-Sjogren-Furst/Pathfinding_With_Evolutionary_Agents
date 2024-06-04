using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;

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

        mapController.ChangeMapParameters(customMaps.GetCustomMap(0));

    }


    //Run backend here

    public MyGameManager()
    {
        customMaps = new();
        MapModel mapModel = customMaps.GetCustomMap(6);
        mapController = new MapController(mapModel);
        AgentModel agentModel = new(1, mapModel.map, mapModel.spawnPoint, mapModel.checkPoints);
        agentController = new AgentController(agentModel);
        HPAGraphController = DynamicGraphoperations.InitialiseHPAStar(mapModel.map);
        mmasGraphController = DynamicGraphoperations.InitialiseMMMAS();
        agentController.Scan(agentController.agentModel.agents[0]);
        agentController.UpdateFrontierPoints();
        agentController.UpdateFrontier();




        // foreach(KeyValuePair<int,HashSet<Cluster>> hpaGrah in HPAGraphController._graphModel.ClusterByLevel) {


        // } 

        // HPAGraphController.HierarchicalSearch(new Vector2Int(5, 5), new Vector2Int(50, 50), 1);





    }

    public MyGameManager(MapModel mapModel)
    {
        mapController = new MapController(mapModel);
        AgentModel agentModel = new(1, mapModel.map, mapModel.spawnPoint, mapModel.checkPoints);
        agentController = new AgentController(agentModel);
        HPAGraphController = DynamicGraphoperations.InitialiseHPAStar(mapModel.map);
        mmasGraphController = DynamicGraphoperations.InitialiseMMMAS();
    }








}

