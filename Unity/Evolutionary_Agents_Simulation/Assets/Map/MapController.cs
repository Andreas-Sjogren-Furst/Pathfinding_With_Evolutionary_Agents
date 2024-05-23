using System.Collections.Generic;
using System.Linq;
using Codice.CM.WorkspaceServer.Tree;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class MapController
{
    public MapModel mapModel;

    public MapController(MapModel mapModel){
        this.mapModel = mapModel;
        Initialise();
    }

    private void Initialise(){
        mapModel.checkPoints = GenerateCheckPoints(mapModel.numberOfCheckPoints, mapModel.width, mapModel.height);
        List<MapObject> objects = new(){mapModel.spawnPoint,mapModel.spawnPoint};
        int[,] map2D = CellularAutomata.Create2DMap(mapModel.height, mapModel.width, mapModel.density, mapModel.iterations, mapModel.erosionLimit);
        map2D = RemoveWallsAroundObjects(map2D, objects, mapModel.checkPointSpacing);
        mapModel.map = CreateMap3D(map2D);
    }

    public MapModel GetMapModel(){
        return mapModel;
    }

    public void ChangeMapParameters(MapModel customMap){
        mapModel.numberOfCheckPoints = customMap.numberOfCheckPoints;
        mapModel.density = customMap.density;
        mapModel.iterations = customMap.iterations;
        mapModel.width = customMap.width;
        mapModel.height = customMap.height;
        mapModel.randomSeed = customMap.randomSeed;
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
            mapModel.checkPoints.Add(checkPoint);
        }
    }
    public MapModel ChangeSpawnPoint(MapModel mapModel, AgentSpawnPoint agentSpawnPoint){
        mapModel.spawnPoint = agentSpawnPoint;
        return mapModel;
    }
    
    private MapObject[,] CreateMap3D(int[,] map2D){
        int width = map2D.GetLength(0);
        int height = map2D.GetLength(1);
        MapObject[,] Map3D = new MapObject[height, width];
        for(int i = 0; i < height; i++){
            for(int j = 0; j < width; j++)
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
            int xPos = Random.Range(0, width);
            int yPos = Random.Range(0, height);
            CheckPoint checkPoint = new(new Vector2Int(xPos,yPos));
            checkPoints.Add(checkPoint);
        } return checkPoints;
    }
}