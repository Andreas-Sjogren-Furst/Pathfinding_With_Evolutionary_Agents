using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MapModel 
{
    public readonly int checkPointSpacing = 3;
    public readonly int erosionLimit = 4;
    public int randomSeed;
    public int numberOfCheckPoints;
    public float density;
    public int iterations;
    public int width;
    public int height;
    public MapObject[,] map;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    public MapModel(float density, int iterations, int mapSize, int numberOfCheckPoints, int randomSeed){
        this.density = density;
        this.iterations = iterations;
        this.numberOfCheckPoints = numberOfCheckPoints;
        width = mapSize;
        height = mapSize;
        this.randomSeed = randomSeed;
        map = new MapObject[mapSize,mapSize];
        checkPoints = new();
        spawnPoint = new(new Vector2Int(mapSize/2, mapSize/2));      
    }

}
