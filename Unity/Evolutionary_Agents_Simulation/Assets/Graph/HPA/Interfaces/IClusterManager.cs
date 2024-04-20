using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IClusterManager
{
    HashSet<Cluster> BuildClusters(int level, int[,] globalTileMap);
    Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize);

    Cluster MergeClusters(List<Cluster> clustersToMerge, int level);

    Cluster IncreaseSingleClusterLevel(Cluster cluster);

    Cluster DetermineCluster(HPANode n, int level);

}