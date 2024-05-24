using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintViewModel
{
    public MapObject[,] map;
    public Agent[] agents;
    public IGraphModel hpaGraph;
    public Graph mmasGraph;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;

    public PrintViewModel(MapObject[,] map, Agent[] agents, IGraphModel hpaGraph, Graph mmasGraph, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        this.map = map;
        this.agents = agents;
        this.hpaGraph = hpaGraph;
        this.mmasGraph = mmasGraph;
        this.checkPoints = checkPoints;
        this.spawnPoint = spawnPoint;
    }
}
