using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
using UnityEngine;

public class ScreenViewModel
{
    public MapObject[,] Map;
    public Agent[] Agents;
    public GraphModel graph;
    public bool showGraph;
    public bool showCheckPoint;
    public bool showSpawnPoint;

}
