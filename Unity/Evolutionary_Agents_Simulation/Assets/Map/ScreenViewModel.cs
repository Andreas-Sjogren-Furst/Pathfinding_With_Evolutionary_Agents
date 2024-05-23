using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public class ScreenViewModel
{
    public MapObject[,] map;
    public Agent[] agents;
    public IGraphModel graph;
    public List<CheckPoint> checkPoints;
    public AgentSpawnPoint spawnPoint;

    public ScreenViewModel(MapObject[,] map, Agent[] agents, IGraphModel graph, List<CheckPoint> checkPoints, AgentSpawnPoint spawnPoint){
        this.map = map;
        this.agents = agents;
        this.graph = graph;
        this.checkPoints = checkPoints;
        this.spawnPoint = spawnPoint;
    }   
}
