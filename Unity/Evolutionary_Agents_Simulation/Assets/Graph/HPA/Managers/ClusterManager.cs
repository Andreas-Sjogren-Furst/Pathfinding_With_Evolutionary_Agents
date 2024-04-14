using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ClusterManager : IClusterManager
{
    private readonly INodeManager _nodeManager;
    private readonly IGraphModel _graphModel;


    public ClusterManager(IGraphModel graphModel, INodeManager nodeManager)
    {
        _graphModel = graphModel;

        _nodeManager = nodeManager;
    }

    public HashSet<Cluster> BuildClusters(int level, int[,] globalTileMap)
    {
        HashSet<Cluster> clusters = new HashSet<Cluster>();
        int clusterSize = 10 * level;

        int gridHeight = (int)Math.Ceiling((double)globalTileMap.GetLength(0) / clusterSize);
        int gridWidth = (int)Math.Ceiling((double)globalTileMap.GetLength(1) / clusterSize);

        for (int i = 0; i < gridHeight; i++)
        {
            for (int j = 0; j < gridWidth; j++)
            {
                int startX = i * clusterSize;
                int startY = j * clusterSize;
                int endX = Math.Min(startX + clusterSize - 1, globalTileMap.GetLength(0) - 1);
                int endY = Math.Min(startY + clusterSize - 1, globalTileMap.GetLength(1) - 1);

                Cluster cluster = new Cluster(
                    bottomLeftPos: new Vector2Int(startX, startY),
                    topRightPos: new Vector2Int(endX, endY),
                    level: level,
                    hPANodes: new HashSet<HPANode>(),
                    entrances: new HashSet<Entrance>()
                );

                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        if (globalTileMap[x, y] != 1)
                        {
                            HPANode newNode = _nodeManager.FindOrCreateNode(x, y, cluster);
                            cluster.Nodes.Add(newNode);
                        }
                    }
                }

                cluster = CreateEdgesForCluster(cluster, startX, startY, endX, endY, clusterSize);
                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    public Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize)
    {
        Dictionary<Vector2Int, HPANode> nodeLookup = new Dictionary<Vector2Int, HPANode>();
        foreach (var node in cluster.Nodes)
        {
            nodeLookup[new Vector2Int(node.Position.x, node.Position.y)] = node;
        }

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        foreach (HPANode node in cluster.Nodes)
        {
            int x = node.Position.x;
            int y = node.Position.y;

            for (int direction = 0; direction < 4; direction++)
            {
                int newX = x + dx[direction];
                int newY = y + dy[direction];
                Vector2Int newPosition = new Vector2Int(newX, newY);

                if (newX >= startX && newX <= endX && newY >= startY && newY <= endY && nodeLookup.TryGetValue(newPosition, out HPANode adjacentNode))
                {
                    HPAEdge edge = new HPAEdge(
                        node1: node,
                        node2: adjacentNode,
                        weight: 1,
                        level: cluster.Level,
                        type: HPAEdgeType.INTRA
                    );
                    node.Edges.Add(edge);
                }
            }
        }

        return cluster;
    }

    public Cluster DetermineCluster(HPANode n, int level)
    {
        foreach (Cluster cluster in _graphModel.ClusterByLevel[level])
        {
            if (cluster.Contains(n.Position))
            {
                return cluster;
            }
        }
        return null;
    }
}