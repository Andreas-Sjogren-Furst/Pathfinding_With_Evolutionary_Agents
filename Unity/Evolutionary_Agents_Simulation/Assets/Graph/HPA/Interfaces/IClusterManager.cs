// written by: Gustav Clausen s214940
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IClusterManager
{
    HashSet<Cluster> BuildClusters(int level, MapObject[,] globalTileMap, int cluserSize);
    // Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize);
    public Cluster CreateEdgesForCluster(Cluster cluster, Vector2Int newNodePosition, Boolean dynamicallyAddInterEdges = false);
    Cluster MergeClusters(List<Cluster> clustersToMerge, int level);

    Cluster IncreaseSingleClusterLevel(Cluster cluster);

    Cluster DetermineCluster(Vector2Int n, int level);

}