using System.Collections.Generic;

public class ScreenViewModel
{
    public int accessibleNodes;
    public MapObject[,] map;
    public Agent[] agents;
    public HashSet<Point> visibleTiles;
    public HashSet<Point> frontierPoints;
    public HashSet<Point> frontier;
    public List<HashSet<Cluster>> hpaGraphs;
    public IGraphModel hpaGraph;
    public Graph mmasGraph;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    

    public ScreenViewModel(int accessibleNodes, HashSet<Point> visibleTiles, HashSet<Point> frontierPoints, HashSet<Point> frontier, MapObject[,] map, Agent[] agents,IGraphModel hpaGraph, Graph mmasGraph, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        this.accessibleNodes = accessibleNodes;
        this.visibleTiles = visibleTiles;
        this.frontierPoints = frontierPoints;
        this.frontier = frontier;
        this.map = map;
        this.agents = agents;
        this.hpaGraph = hpaGraph;
        this.mmasGraph = mmasGraph;
        this.checkPoints = checkPoints;
        this.spawnPoint = spawnPoint;
    }   
}
