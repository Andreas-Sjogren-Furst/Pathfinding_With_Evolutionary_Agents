using System.Collections.Generic;
// Written by: Andreas Sjögren Fürst (s201189)
public class ScreenViewModel
{
    public int accessibleNodes;
    public MapObject[,] map;
    public Agent[] agents;
    public HashSet<Point> visibleTiles;
    public List<Point> frontierPointsForRendering;
    public HashSet<Point> centroidsForRendering;
    public List<HashSet<Cluster>> hpaGraphs;
    public IGraphModel hpaGraph;
    public Graph mmasGraph;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;
    

    public ScreenViewModel(int accessibleNodes, HashSet<Point> visibleTiles, List<Point> frontierPointsForRendering, HashSet<Point> centroidsForRendering, MapObject[,] map, Agent[] agents,IGraphModel hpaGraph, Graph mmasGraph, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        this.accessibleNodes = accessibleNodes;
        this.visibleTiles = visibleTiles;
        this.frontierPointsForRendering = frontierPointsForRendering;
        this.centroidsForRendering = centroidsForRendering;
        this.map = map;
        this.agents = agents;
        this.hpaGraph = hpaGraph;
        this.mmasGraph = mmasGraph;
        this.checkPoints = checkPoints;
        this.spawnPoint = spawnPoint;
    }   
}
