using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapController
{
    public MapModel mapModel;
    public MapController(MapModel mapModel){
        this.mapModel = mapModel;
        InitMap(this.mapModel);
    }

    public void ClearCheckPoints(){
        mapModel.CheckPoints.Clear();
    }

    public void ClearObjectsFromMap(List<string> objectTags){
        ObjectRemover.ClearObjects(objectTags, mapModel.Map);
    }

    public void InitMap(MapModel mapModel){
        int[,] map2D = CellularAutomata.Create2DMap(mapModel);
        mapModel.Map = CreateMap3D(map2D);
        mapModel.SpawnPoint = GenerateSpawnPoint();
        mapModel.CheckPoints = GenerateCheckPoints(mapModel.NumberOfCheckPoints);
        List<MapObject> objects = mapModel.CheckPoints.Cast<MapObject>().ToList();
        objects.Add(mapModel.SpawnPoint);
        mapModel.Map = RemoveWallsAroundObjects(mapModel.Map, objects, mapModel.CheckPointSpacing);
    }

    public void ShowCheckPoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            checkPoint.Spawn();
        }
    }

    public void ShowSpawnPoint(AgentSpawnPoint agentSpawnPoint){
        agentSpawnPoint.Spawn();
    }

    private MapObject[,] RemoveWallsAroundObjects(MapObject[,] map, List<MapObject> mapObjects, int spacing){
        int mapHeight = mapModel.mapHeight;
        int mapWidth = mapModel.mapWidth;
        foreach(MapObject mapObject in mapObjects){
            for(int i = mapObject.ArrayPosition.y - spacing; i < 2 * spacing; i++){
                for(int j = mapObject.ArrayPosition.x - spacing; j < 2 * spacing; j++){
                    if(i >= 0 && i < mapHeight && j >= 0 && j < mapWidth) 
                        map[i,j] = new Tile(new Vector2Int(i,j));
                }
            }
        } return map;
    }
    public void ShowMap(){
        ShowObjects(mapModel.Map);
    }

    public void AddCheckpoints(List<CheckPoint> checkPoints){
        foreach(CheckPoint checkPoint in checkPoints){
            mapModel.CheckPoints.Add(checkPoint);
        }
    }
    public void ChangeSpawnPoint(AgentSpawnPoint agentSpawnPoint){
        mapModel.SpawnPoint = agentSpawnPoint;
    }

    private AgentSpawnPoint GenerateSpawnPoint(){
        int xPos = Random.Range(0, mapModel.mapWidth + 1);
        int zPos = Random.Range(0, mapModel.mapHeight + 1);
        Vector2Int spawnPoint = new(xPos, zPos);
        AgentSpawnPoint newSpawnPoint = new(spawnPoint);
        return newSpawnPoint;
    }
    

    public void ChangeMapParameters(MapModel newMapModel){
        mapModel.Density = newMapModel.Density;
        mapModel.NumberOfCheckPoints = newMapModel.NumberOfCheckPoints;
        mapModel.CellularIterations = newMapModel.CellularIterations;
        mapModel.CheckPointSpacing = newMapModel.CheckPointSpacing;
        mapModel.ErosionLimit = newMapModel.ErosionLimit;
        mapModel.RandomSeed = newMapModel.RandomSeed;
    }
    
    private void ShowObjects(MapObject[,] map){
        for(int i = 0; i < map.GetLength(0); i++){
            for(int j = 0; j < map.GetLength(1); j++)
                map[i,j].Spawn();
        }
    }
    private MapObject[,] CreateMap3D(int[,] map2D){
        MapObject[,] tempMap = new MapObject[map2D.GetLength(0), map2D.GetLength(1)];
        for(int i = 0; i < map2D.GetLength(0); i++){
            for(int j = 0; j < map2D.GetLength(1); j++)
            {
                tempMap[i,j] = MapObject.CreateObjectFromType(map2D[i,j],i,j);
            }
        } return tempMap;
    }

    private List<CheckPoint> GenerateCheckPoints(int numberOfCheckPoints)
    {
        List<CheckPoint> checkPoints = new();
        for (int i = 0; i < numberOfCheckPoints; i++)
        {
            int xPos = UnityEngine.Random.Range(0, mapModel.mapWidth + 1);
            int zPos = UnityEngine.Random.Range(0, mapModel.mapHeight + 1);
            CheckPoint checkPoint = new(new Vector2Int(xPos, zPos));
            checkPoints.Add(checkPoint);
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