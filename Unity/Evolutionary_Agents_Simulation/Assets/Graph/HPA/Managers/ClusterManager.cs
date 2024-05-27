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

    public HashSet<Cluster> BuildClusters(int level, MapObject[,] globalTileMap)
    {
        HashSet<Cluster> clusters = new HashSet<Cluster>();
        int clusterSize = 10 * level;

        int gridWidth = (int)Math.Ceiling((double)globalTileMap.GetLength(1) / clusterSize);
        int gridHeight = (int)Math.Ceiling((double)globalTileMap.GetLength(0) / clusterSize);

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                int startX = i * clusterSize;
                int startY = j * clusterSize;
                int endX = Math.Min(startX + clusterSize - 1, globalTileMap.GetLength(1) - 1);
                int endY = Math.Min(startY + clusterSize - 1, globalTileMap.GetLength(0) - 1);

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
                        if (globalTileMap[x, y].Type != MapObject.ObjectType.Wall)
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



                    _edgeManager.AddHPAEdge(n1, n2, entrance.Edge1.Weight, level, HPAEdgeType.INTER, IntraPath: entrance.Edge1.IntraPaths);

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
        // Debug.Log("Increasing cluster level from " + c.Level);

        // Assuming new boundaries might need calculation or adjustment
        Vector2Int newBottomLeft = c.bottomLeftPos; // These might need to be recalculated
        Vector2Int newTopRight = c.topRightPos;     // These might need to be recalculated

        Cluster newCluster = new Cluster(c.Level + 1, new HashSet<HPANode>(), new HashSet<Entrance>(), newBottomLeft, newTopRight);
        _graphModel.ClusterByLevel[c.Level + 1].Add(newCluster);

        foreach (HPANode n in c.Nodes)
        {
            // Check if node should logically continue to the next level cluster
            if (c.Contains(n.Position))
            {
                HPANode newNode = _nodeManager.FindOrCreateNode(n.Position.x, n.Position.y, newCluster);
                TransferEdges(n, newNode, newCluster);
                newCluster.Nodes.Add(newNode);
            }
            else
            {
                // Debug.Log($"Node at {n.Position} is outside the new cluster bounds and won't be transferred.");
            }
        }

        TransferEntrances(c, newCluster);

        return newCluster;
    }

    private void TransferEdges(HPANode originalNode, HPANode newNode, Cluster newCluster)
    {
        foreach (var edge in originalNode.Edges)
        {
            HPANode node2 = _nodeManager.FindOrCreateNode(edge.Node2.Position.x, edge.Node2.Position.y, newCluster);
            if (!newNode.Edges.Any(e => e.Node2 == node2))
            {
                _edgeManager.AddHPAEdge(newNode, node2, edge.Weight, newCluster.Level, edge.Type, IntraPath: edge.IntraPaths);
                // Debug.Log($"Edge transferred from {newNode.Position} to {node2.Position} at new level {newCluster.Level}");
            }
        }
    }

    private void TransferEntrances(Cluster originalCluster, Cluster newCluster)
    {
        foreach (Entrance e in originalCluster.Entrances)
        {
            // Assume IsNodeOnBoundary is a new method to check if the node is at the boundary of the cluster
            if (IsNodeOnBoundary(e.Node1, newCluster) || IsNodeOnBoundary(e.Node2, newCluster))
            {
                HPANode n1 = _nodeManager.FindOrCreateNode(e.Node1.Position.x, e.Node1.Position.y, newCluster);
                HPANode n2 = _nodeManager.FindOrCreateNode(e.Node2.Position.x, e.Node2.Position.y, newCluster);
                Entrance newEntrance = new Entrance(newCluster, newCluster, n1, n2);
                newCluster.Entrances.Add(newEntrance);
                _graphModel.EntrancesByLevel[newCluster.Level].Add(newEntrance);
                // Debug.Log("Entrance transferred to new level.");
            }
            else
            {
                // Debug.Log($"Entrance between {e.Node1.Position} and {e.Node2.Position} not valid for new cluster at level {newCluster.Level}. Nodes may not be on boundary.");
            }
        }
    }

    private bool IsNodeOnBoundary(HPANode node, Cluster cluster)
    {
        return (node.Position.x == cluster.bottomLeftPos.x || node.Position.x == cluster.topRightPos.x ||
                node.Position.y == cluster.bottomLeftPos.y || node.Position.y == cluster.topRightPos.y);
    }











}