using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphModel : IGraphModel
{
    public int[,] GlobalTileMap { get; set; }

    public Dictionary<int, HashSet<Entrance>> EntrancesByLevel { get; set; } // E 
    public Dictionary<int, HashSet<Cluster>> ClusterByLevel { get; set; }
    public Dictionary<int, Dictionary<Vector2Int, HPANode>> NodesByLevel { get; set; }

    public GraphModel(int[,] globalTileMap)
    {
        GlobalTileMap = globalTileMap;
        EntrancesByLevel = new Dictionary<int, HashSet<Entrance>>();
        ClusterByLevel = new Dictionary<int, HashSet<Cluster>>();
        NodesByLevel = new Dictionary<int, Dictionary<Vector2Int, HPANode>>();
    }
}