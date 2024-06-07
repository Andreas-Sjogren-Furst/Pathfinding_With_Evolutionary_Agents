using System.Collections.Generic;
public class AgentModel
{
    public int amountOfAgents;
    public Agent[] agents;
    public MapObject[,] map;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    public HashSet<Point> visibleTiles;
    public HashSet<Point> visibleWalls;
    public HashSet<Point> centroidsForRendering;
    public List<Point> frontierPointsForRendering;
    public HashSet<Point> frontierPoints;
    public HashSet<Point> centroids;
    public Point[] currentCentroidsInFocus;
    public Stack<Point> bestTour;

    public AgentModel(int amountOfAgents, MapObject[,] map, AgentSpawnPoint spawnPoint, List<CheckPoint> checkPoints)
    {
        this.amountOfAgents = amountOfAgents;
        this.map = map;
        this.spawnPoint = spawnPoint;
        this.checkPoints = checkPoints;
        centroidsForRendering = new();
        frontierPointsForRendering = new();
        visibleTiles = new();
        visibleWalls = new();
        frontierPoints = new();
        centroids = new();
        bestTour = new();
        currentCentroidsInFocus = new Point[amountOfAgents];
        agents = new Agent[amountOfAgents];
        for (int i = 0; i < amountOfAgents; i++) { agents[i] = new Agent(spawnPoint.ArrayPosition, i); }
    }
}
