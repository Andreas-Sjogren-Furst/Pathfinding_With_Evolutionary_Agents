using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ClusterManager : IClusterManager
{
    private readonly INodeManager _nodeManager;
    private readonly IGraphModel _graphModel;

    private readonly IEdgeManager _edgeManager;

    private readonly IEntranceManager _entranceManager;


    public ClusterManager(IGraphModel graphModel, INodeManager nodeManager, IEdgeManager edgeManager, IEntranceManager entranceManager)
    {
        _graphModel = graphModel;

        _nodeManager = nodeManager;
        _edgeManager = edgeManager;
        _entranceManager = entranceManager;
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

                // cluster = CreateEdgesForCluster(cluster, startX, startY, endX, endY, clusterSize);
                clusters.Add(cluster);
            }
        }

        return clusters;
    }

    // public Cluster CreateEdgesForCluster(Cluster cluster, int startX, int startY, int endX, int endY, int clusterSize)
    // {
    //     Dictionary<Vector2Int, HPANode> nodeLookup = new Dictionary<Vector2Int, HPANode>();
    //     foreach (var node in cluster.Nodes)
    //     {
    //         nodeLookup[new Vector2Int(node.Position.x, node.Position.y)] = node;
    //     }

    //     int[] dx = { 1, -1, 0, 0 };
    //     int[] dy = { 0, 0, 1, -1 };

    //     foreach (HPANode node in cluster.Nodes)
    //     {
    //         int x = node.Position.x;
    //         int y = node.Position.y;

    //         for (int direction = 0; direction < 4; direction++)
    //         {
    //             int newX = x + dx[direction];
    //             int newY = y + dy[direction];
    //             Vector2Int newPosition = new Vector2Int(newX, newY);

    //             if (newX >= startX && newX <= endX && newY >= startY && newY <= endY && nodeLookup.TryGetValue(newPosition, out HPANode adjacentNode))
    //             {
    //                 HPAEdge edge = new HPAEdge(
    //                     node1: node,
    //                     node2: adjacentNode,
    //                     weight: 1,
    //                     level: cluster.Level,
    //                     type: HPAEdgeType.INTRA
    //                 );
    //                 node.Edges.Add(edge);
    //             }
    //         }
    //     }

    //     return cluster;
    // }

    public Cluster CreateEdgesForCluster(Cluster cluster, Vector2Int newNodePosition, Boolean dynamicallyAddInterEdges = false) // kan skiftes ud med createEdgeForCluster, men et parameter skal ændres ift. om den også skal lave inter.
    {
        foreach (var direction in new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) })
        {
            Vector2Int adjacentPosition = newNodePosition + direction;
            if (cluster.Contains(adjacentPosition) && _graphModel.NodesByLevel[1].ContainsKey(adjacentPosition))
            {
                _edgeManager.AddHPAEdge(_graphModel.NodesByLevel[1][newNodePosition], _graphModel.NodesByLevel[1][adjacentPosition], 1, 1, HPAEdgeType.INTRA);
            }
            else if (!cluster.Contains(adjacentPosition) && _graphModel.NodesByLevel[1].ContainsKey(adjacentPosition))
            {
                Cluster adjacentCluster = DetermineCluster(adjacentPosition, 1);

                if (dynamicallyAddInterEdges && adjacentCluster.isOnBorder(adjacentPosition))
                { // if the other node is on the border, we create a inter edge TOOD: perhaps we should create entrance instead. 
                    _edgeManager.AddHPAEdge(_graphModel.NodesByLevel[1][newNodePosition], _graphModel.NodesByLevel[1][adjacentPosition], 1, 1, HPAEdgeType.INTER);
                }



            }
        }
        return cluster;
    }

    public Cluster DetermineCluster(Vector2Int n, int level)
    {
        foreach (Cluster cluster in _graphModel.ClusterByLevel[level])
        {
            if (cluster.Contains(n))
            {
                return cluster;
            }
        }
        return null;
    }


    public Cluster MergeClusters(List<Cluster> clustersToMerge, int level)
    {


        Cluster mergedCluster = new Cluster(level, new HashSet<HPANode>(), new HashSet<Entrance>(), new Vector2Int(int.MaxValue, int.MaxValue), new Vector2Int(int.MinValue, int.MinValue));
        _graphModel.ClusterByLevel[level].Add(mergedCluster);

        // Determine the boundaries of the new merged cluster
        Vector2Int bottomLeft = new Vector2Int(clustersToMerge.Min(c => c.bottomLeftPos.x), clustersToMerge.Min(c => c.bottomLeftPos.y));
        Vector2Int topRight = new Vector2Int(clustersToMerge.Max(c => c.topRightPos.x), clustersToMerge.Max(c => c.topRightPos.y));

        mergedCluster.bottomLeftPos = bottomLeft;
        mergedCluster.topRightPos = topRight;





        // Merge all clusters
        foreach (Cluster cluster in clustersToMerge)
        {
            // Add all entrances
            foreach (Entrance entrance in cluster.Entrances)
            {


                HPANode n1 = _nodeManager.FindOrCreateNode(entrance.Node1.Position.x, entrance.Node1.Position.y, mergedCluster);
                HPANode n2 = _nodeManager.FindOrCreateNode(entrance.Node2.Position.x, entrance.Node2.Position.y, mergedCluster);


                if (clustersToMerge.Contains(entrance.Node1.Cluster) && clustersToMerge.Contains(entrance.Node2.Cluster)) // adding intra edges. 
                {
                    mergedCluster.Nodes.Add(n1);
                    mergedCluster.Nodes.Add(n2);
                }
                else if (clustersToMerge.Contains(entrance.Node1.Cluster) && !clustersToMerge.Contains(entrance.Node2.Cluster)) // adding inter edges between clusters. 
                {



                    _edgeManager.AddHPAEdge(n1, n2, entrance.Edge1.Weight, level, HPAEdgeType.INTER);

                    Entrance entrance1 = new Entrance(n1.Cluster, n2.Cluster, n1, n2); //TODO: check if entrance should be reversed? 
                    Entrance entrance2 = new Entrance(n2.Cluster, n1.Cluster, n2, n1);


                    mergedCluster.Entrances.Add(entrance1);
                    mergedCluster.Entrances.Add(entrance2);
                    _graphModel.EntrancesByLevel[level].Add(entrance1);
                    _graphModel.EntrancesByLevel[level].Add(entrance2);
                    mergedCluster.Nodes.Add(n1); // only add the node inside the cluster to the merged cluster.

                }

                // update the cluster references of the nodes. 

                if (mergedCluster.Contains(n1.Position))
                {
                    n1.Cluster = mergedCluster;

                }
                if (mergedCluster.Contains(n2.Position))
                {
                    n2.Cluster = mergedCluster;

                }
            }


        }





        // Create the new merged cluster
        return mergedCluster;
    }



    public Cluster IncreaseSingleClusterLevel(Cluster c)
    {
        Cluster newCluster = new Cluster(c.Level + 1, new HashSet<HPANode>(), new HashSet<Entrance>(), c.bottomLeftPos, c.topRightPos);
        _graphModel.ClusterByLevel[c.Level + 1].Add(newCluster);

        foreach (HPANode n in c.Nodes)
        {
            HPANode newNode = _nodeManager.FindOrCreateNode(n.Position.x, n.Position.y, newCluster);
            newNode.Merge(n);
            newCluster.Nodes.Add(newNode);
        }

        foreach (Entrance e in c.Entrances)
        {
            HPANode n1 = _nodeManager.FindOrCreateNode(e.Node1.Position.x, e.Node1.Position.y, newCluster);
            HPANode n2 = _nodeManager.FindOrCreateNode(e.Node2.Position.x, e.Node2.Position.y, newCluster);
            Entrance newEntrance = new Entrance(newCluster, newCluster, n1, n2);
            newCluster.Entrances.Add(newEntrance);
            _graphModel.EntrancesByLevel[c.Level + 1].Add(newEntrance);
        }

        return newCluster;
    }





}