using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController
{
    public MapModel mapModel;
    public TileConfig tileConfig;
    public WallConfig wallConfig;
    public CheckPointConfig checkPointConfig;
    public SpawnPointConfig spawnPointConfig;
    public MapController(MapModel mapModel, TileConfig tileConfig, WallConfig wallConfig, CheckPointConfig checkPointConfig, SpawnPointConfig spawnPointConfig){
        this.mapModel = mapModel;
        this.tileConfig = tileConfig;
        this.wallConfig = wallConfig;
        this.checkPointConfig = checkPointConfig;
        this.spawnPointConfig = spawnPointConfig;
    }

    public void ClearCheckPoints(){
        mapModel.CheckPoints.Clear();
    }

    public void ClearObjectsFromMap(List<string> objectTags){
        ObjectRemover.ClearObjects(objectTags, mapModel.Map);
    }

    public void InitMap(){
        int[,] map2D = CellularAutomata.Create2DMap(mapModel);
        mapModel.Map = CreateMap3D(map2D);
        mapModel.SpawnPoint = GenerateSpawnPoint();
        mapModel.CheckPoints = GenerateCheckPoints(mapModel.NumberOfCheckPoints);
        //List<MapObject> objects = mapModel.CheckPoints.Cast<MapObject>().ToList();
        //objects.Add(mapModel.SpawnPoint);
        //mapModel.Map = RemoveWallsAroundObjects(mapModel.Map, objects, mapModel.CheckPointSpacing);
    }

    private void RemoveWallsAroundObjects(List<MapObject> mapObjects, int spacing){
        int mapHeight = mapModel.mapHeight;
        int mapWidth = mapModel.mapWidth;
        foreach(MapObject mapObject in mapObjects){
            for(int i = mapObject.ArrayPosition.y - spacing; i < 2 * spacing; i++){
                for(int j = mapObject.ArrayPosition.x - spacing; j < 2 * spacing; j++){
                    if(i >= 0 && i < mapHeight && j >= 0 && j < mapWidth){
                        Tile tile = new();
                        ObjectRemover.DestroyObject(mapModel.Map[i,j].Object);
                        mapModel.Map[i,j] = tile.Create(new Vector2Int(i,j),tileConfig);
                    } 
                }
            }
        } 
    }

    // TODO: private void addNewObjectToMap
    
    public void AddCheckpoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            mapModel.CheckPoints.Add(checkPoint);
        }
    }
    public void ChangeSpawnPoint(AgentSpawnPoint agentSpawnPoint){
        mapModel.SpawnPoint = agentSpawnPoint;
    }

    private AgentSpawnPoint GenerateSpawnPoint(){
        AgentSpawnPoint agentSpawnPoint = new();
        int xPos = Random.Range(0, mapModel.mapWidth + 1);
        int zPos = Random.Range(0, mapModel.mapHeight + 1);
        Vector2Int spawnPoint = new(xPos, zPos);
        return agentSpawnPoint.Create(spawnPoint, spawnPointConfig);
    }
    

    public void ChangeMapParameters(MapModel newMapModel){
        mapModel.Density = newMapModel.Density;
        mapModel.NumberOfCheckPoints = newMapModel.NumberOfCheckPoints;
        mapModel.CellularIterations = newMapModel.CellularIterations;
        mapModel.CheckPointSpacing = newMapModel.CheckPointSpacing;
        mapModel.ErosionLimit = newMapModel.ErosionLimit;
        mapModel.RandomSeed = newMapModel.RandomSeed;
    }
    
    private MapObject[,] CreateMap3D(int[,] map2D){
        MapObject[,] tempMap = new MapObject[map2D.GetLength(0), map2D.GetLength(1)];
        for(int i = 0; i < map2D.GetLength(0); i++){
            for(int j = 0; j < map2D.GetLength(1); j++)
            {
                tempMap[i,j] = CreateObjectFromType(map2D[i,j],i,j);
            }
        } return tempMap;
    }

    private MapObject CreateObjectFromType(int arrayNumber, int i, int j){
        
        MapObject.ObjectType objectType = (MapObject.ObjectType)arrayNumber;
        Vector2Int arrayPosition = new(i,j);
        switch (objectType)
        {
            case MapObject.ObjectType.Tile:
                Tile tile = new();
                return tile.Create(arrayPosition, tileConfig);

            case MapObject.ObjectType.Wall:
                Wall wall = new();
                return wall.Create(arrayPosition, wallConfig);

            case MapObject.ObjectType.CheckPoint:
                CheckPoint checkPoint = new();
                return checkPoint.Create(arrayPosition, checkPointConfig);

            case MapObject.ObjectType.AgentSpawnPoint:
                AgentSpawnPoint agentSpawnPoint = new();
                return agentSpawnPoint.Create(arrayPosition, spawnPointConfig);

            default:
                throw new System.Exception("Invalid arrayNumber in the method CreateObjectFromType");
        };
    }

    private List<CheckPoint> GenerateCheckPoints(int numberOfCheckPoints)
    {
        List<CheckPoint> checkPoints = new();
        for (int i = 0; i < numberOfCheckPoints; i++)
        {
            CheckPoint checkPoint = new();
            int xPos = Random.Range(0, mapModel.mapWidth + 1);
            int zPos = Random.Range(0, mapModel.mapHeight + 1);
            checkPoints.Add(checkPoint.Create(new Vector2Int(xPos, zPos), checkPointConfig));
        } return checkPoints;
    }

    private bool IsMapValid(MapObject[,] map, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint)
    {   
        foreach (CheckPoint checkPoint in checkPoints){
            if(Astar.FindPath(spawnPoint.ArrayPosition, checkPoint.ArrayPosition, map) == null)
                return false;
        } return true;
    }
}