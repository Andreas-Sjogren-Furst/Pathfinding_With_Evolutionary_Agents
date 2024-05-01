using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public interface IGraphModel
{
    MapObject[,] GlobalTileMap { get; set; }

    Dictionary<int, HashSet<Cluster>> ClusterByLevel { get; }
    Dictionary<int, Dictionary<Vector2Int, HPANode>> NodesByLevel { get; }
    Dictionary<int, HashSet<Entrance>> EntrancesByLevel { get; set; } // E 



}