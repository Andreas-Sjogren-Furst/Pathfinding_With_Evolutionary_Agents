using System;
using UnityEngine;

public class MyGameManager
{    

    // Object for Custom Maps
    CustomMaps customMaps;
        
   
    // Controllers
    public MapController mapController;
    public HPAStar graphController;
    public AgentController agentController;
        

    // Main
    public void main(){
        mapController.ChangeMapParameters(customMaps.GetCustomMap(0));
        
    }
    
    
    //Run backend here

    public MyGameManager(){
        customMaps = new();
        MapModel mapModel = customMaps.GetCustomMap(1);
        mapController = new MapController(mapModel);
        agentController = new AgentController(new AgentModel(1));
        graphController = InitialiseHPAStar(mapModel.map);
    }

    private HPAStar InitialiseHPAStar(MapObject[,] map){
        PathFinder _pathFinder = new PathFinder();
        GraphModel _graphModel = new GraphModel(map);
        IEdgeManager edgeManager = new EdgeManager(_pathFinder);
        NodeManager _nodeManager = new NodeManager(_graphModel, edgeManager);
        IEntranceManager entranceManager = new EntranceManager(_graphModel, _nodeManager);
        IClusterManager clusterManager = new ClusterManager(_graphModel, _nodeManager, edgeManager, entranceManager);
        HPAStar hpaStar = new HPAStar(_graphModel, clusterManager, _nodeManager, entranceManager, edgeManager, _pathFinder);
        hpaStar.Preprocessing(1);
        return hpaStar;
    }
    
}

