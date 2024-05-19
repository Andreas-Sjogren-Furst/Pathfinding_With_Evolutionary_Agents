using System.Collections.Generic;
using System.Linq;
using Codice.CM.WorkspaceServer.Tree;
using UnityEngine;

public class MapController
{
    public MapModel mapModel;

    public MapController(MapModel mapModel){
        this.mapModel = mapModel;
    }

    public void Initialise(){
        mapModel.SpawnPoint = GenerateSpawnPoint(mapModel.Width, mapModel.Height);
        mapModel.CheckPoints = GenerateCheckPoints(mapModel.AmountOfCheckPoints, mapModel.Width, mapModel.Height);
        List<MapObject> objects = new(){mapModel.SpawnPoint,mapModel.SpawnPoint};
        int[,] map2D = CellularAutomata.Create2DMap(mapModel.Height, mapModel.Width, mapModel.Density, mapModel.CellularIterations, mapModel.ErosionLimit);
        map2D = RemoveWallsAroundObjects(map2D, objects, mapModel.CheckPointSpacing);
        mapModel.Map = CreateMap3D(map2D);
    }

    public void SetCustomMap(InitCustomMaps customMap){
        mapModel.AmountOfCheckPoints = customMap.numberOfCheckPoints;
        mapModel.Density = customMap.density;
        mapModel.CellularIterations = customMap.cellularIterations;
        mapModel.Width = customMap.width;
        mapModel.Height = customMap.height;
        mapModel.RandomSeed = customMap.randomSeed;
    }

    private int[,] InitialiseSpawnPoint(AgentSpawnPoint spawnPoint, int[,] map2D){
        map2D[spawnPoint.ArrayPosition.x,spawnPoint.ArrayPosition.y] = (int)spawnPoint.Type;
        return map2D;
    } 
    private int[,] InitialiseCheckPoint(List<CheckPoint> checkPoints, int[,] map2D){
        foreach(CheckPoint checkPoint in checkPoints){
            map2D[checkPoint.ArrayPosition.x,checkPoint.ArrayPosition.y] = (int)checkPoint.Type;
        } return map2D;
    }

    private int[,] RemoveWallsAroundObjects(int[,] map2D, List<MapObject> objects,int spacing){
        int mapHeight = map2D.GetLength(0);
        int mapWidth = map2D.GetLength(1);
        foreach(MapObject mapObject in objects){
            for(int i = mapObject.ArrayPosition.y - spacing; i < 2 * spacing; i++){
                for(int j = mapObject.ArrayPosition.x - spacing; j < 2 * spacing; j++){
                    if(i >= 0 && i < mapHeight && j >= 0 && j < mapWidth){
                        map2D[i,j] = 0;
                    } 
                }
            }
        } return map2D;
    }

    // TODO: private void addNewObjectToMap
    
    public void AddCheckpoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            mapModel.CheckPoints.Add(checkPoint);
        }
    }
    public MapModel ChangeSpawnPoint(MapModel mapModel,AgentSpawnPoint agentSpawnPoint){
        mapModel.SpawnPoint = agentSpawnPoint;
        return mapModel;
    }

    private AgentSpawnPoint GenerateSpawnPoint(int width, int height){
        int xPos = Random.Range(0, width + 1);
        int zPos = Random.Range(0, height + 1);
        Vector2Int spawnPoint = new(xPos, zPos);
        return new AgentSpawnPoint(spawnPoint);
    }
    

    public void ChangeMapParameters(MapModel newMapModel){
        mapModel.Density = newMapModel.Density;
        mapModel.AmountOfCheckPoints = newMapModel.AmountOfCheckPoints;
        mapModel.CellularIterations = newMapModel.CellularIterations;
        mapModel.RandomSeed = newMapModel.RandomSeed;
    }
    
    private MapObject[,] CreateMap3D(int[,] map2D){
        MapObject[,] Map3D = new MapObject[map2D.GetLength(0), map2D.GetLength(1)];
        for(int i = 0; i < map2D.GetLength(0); i++){
            for(int j = 0; j < map2D.GetLength(1); j++)
                Map3D[i,j] = CreateObjectFromType(map2D[i,j],i,j);
        } return Map3D;
    }

    private MapObject CreateObjectFromType(int arrayNumber, int i, int j){
        
        MapObject.ObjectType objectType = (MapObject.ObjectType)arrayNumber;
        Vector2Int arrayPosition = new(i,j);
        switch (objectType)
        {
            case MapObject.ObjectType.Tile:
                return new Tile(arrayPosition);

            case MapObject.ObjectType.Wall:
                return new Wall(arrayPosition);

            case MapObject.ObjectType.CheckPoint:
                return new CheckPoint(arrayPosition);

            case MapObject.ObjectType.AgentSpawnPoint:
                return new AgentSpawnPoint(arrayPosition);
            default:
                throw new System.Exception("Invalid arrayNumber in the method CreateObjectFromType");
        };
    }

    private List<CheckPoint> GenerateCheckPoints(int numberOfCheckPoints, int width, int height)
    {
        List<CheckPoint> checkPoints = new();
        for (int i = 0; i < numberOfCheckPoints; i++)
        {
            int xPos = Random.Range(0, width + 1);
            int yPos = Random.Range(0, height + 1);
            CheckPoint checkPoint = new(new Vector2Int(xPos,yPos));
            checkPoints.Add(checkPoint);
        } return checkPoints;
    }
}