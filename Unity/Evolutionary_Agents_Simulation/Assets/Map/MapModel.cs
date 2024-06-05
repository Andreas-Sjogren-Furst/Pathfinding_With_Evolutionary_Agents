using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class MapModel
{
    public readonly int checkPointSpacing = 3;
    public readonly int erosionLimit = 4;
    public int randomSeed;
    public int amountOfAgents;
    public int numberOfCheckPoints;
    public float density;
    public int iterations;
    public int width;
    public int height;
    public int accessibleNodes;
    public MapObject[,] map;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    public MapModel(float density, int iterations, int mapSize, int numberOfCheckPoints, int amountOfAgents, int randomSeed)
    {
        this.density = density;
        this.iterations = iterations;
        this.numberOfCheckPoints = numberOfCheckPoints;
        accessibleNodes = 0;
        width = mapSize;
        height = mapSize;
        this.randomSeed = randomSeed;
        this.amountOfAgents = amountOfAgents;
        map = new MapObject[mapSize, mapSize];
        checkPoints = new();
        spawnPoint = new(new Vector2Int(mapSize / 2, mapSize / 2));
    }

}
