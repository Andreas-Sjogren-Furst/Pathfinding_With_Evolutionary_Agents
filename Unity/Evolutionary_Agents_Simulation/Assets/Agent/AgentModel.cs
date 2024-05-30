using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentModel
{
    public int amountOfAgents;
    public Agent[] agents;
    public MapObject[,] map;
    public AgentSpawnPoint spawnPoint;
    public HashSet<Point> visibleTiles;
    public HashSet<Point> visibleWalls;
    // Stack<Vector2Int> frontierPoints;
    
    public AgentModel(int amountOfAgents, MapObject[,] map, AgentSpawnPoint spawnPoint){
        this.amountOfAgents = amountOfAgents;
        this.map = map;
        this.spawnPoint = spawnPoint;
        visibleTiles = new();
        visibleWalls = new();
        agents = new Agent[amountOfAgents];
        for(int i = 0; i < amountOfAgents; i++){ agents[i] = new Agent(spawnPoint.ArrayPosition,i); }
    }
}
