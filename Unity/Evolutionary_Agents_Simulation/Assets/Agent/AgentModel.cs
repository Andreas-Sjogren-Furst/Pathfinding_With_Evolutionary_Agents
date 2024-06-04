using System.Collections.Generic;

public class AgentModel
{
    public int amountOfAgents;
    public Agent[] agents;
    public MapObject[,] map;
    public List<Point> accesibleCheckpoints; 
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    public HashSet<Point> visibleTiles;
    public HashSet<Point> visibleWalls;
    public HashSet<Point> frontierPoints;
    public Dictionary<int,Point> centroids;
    
    public AgentModel(int amountOfAgents, MapObject[,] map, AgentSpawnPoint spawnPoint, List<CheckPoint> checkPoints){
        this.amountOfAgents = amountOfAgents;
        this.map = map;
        this.spawnPoint = spawnPoint;
        this.checkPoints = checkPoints;
        accesibleCheckpoints = new();
        visibleTiles = new();
        visibleWalls = new();
        frontierPoints = new();
        centroids = new();
        agents = new Agent[amountOfAgents];
        for(int i = 0; i < amountOfAgents; i++){ agents[i] = new Agent(spawnPoint.ArrayPosition,i); }
    }
}
