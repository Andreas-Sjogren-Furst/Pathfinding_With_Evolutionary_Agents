using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class HPAStar : IHPAStar
{
    private readonly IGraphModel _graphModel;
    private readonly IClusterManager _clusterManager;
    private readonly INodeManager _nodeManager;
    private readonly IEntranceManager _entranceManager;


    private readonly IPathFinder _pathFinder;

    private readonly IEdgeManager _edgeManager;

    public HPAStar(IGraphModel graphModel, IClusterManager clusterManager, INodeManager nodeManager, IEntranceManager entranceManager, IEdgeManager edgeManager, IPathFinder pathFinder)
    {
        _graphModel = graphModel;
        _clusterManager = clusterManager;
        _nodeManager = nodeManager;
        _entranceManager = entranceManager;
        _edgeManager = edgeManager;
        _pathFinder = pathFinder;

    }
    public void Preprocessing(int maxLevel)
    {
        AbstractMaze();
        BuildGraph();
        for (int l = 2; l <= maxLevel; l++)
        {
            AddLevelToGraph(l);
        }
    }

    public void AbstractMaze()
    {
        _graphModel.ClusterByLevel.Add(1, _clusterManager.BuildClusters(1, _graphModel.GlobalTileMap));
        foreach (Cluster c1 in _graphModel.ClusterByLevel[1])
        {
            foreach (Cluster c2 in _graphModel.ClusterByLevel[1])
            {
                if (Adjacent(c1, c2))
                {
                    HashSet<Entrance> entrances = _entranceManager.BuildEntrances(c1, c2);
                    c1.Entrances.UnionWith(entrances);
                    c2.Entrances.UnionWith(entrances);
                    if (!_graphModel.EntrancesByLevel.ContainsKey(1))
                    {
                        _graphModel.EntrancesByLevel.Add(1, new HashSet<Entrance>());
                    }
                    _graphModel.EntrancesByLevel[1].UnionWith(entrances);
                }
            }
        }
    }

    public void BuildGraph()
    {
        foreach (Entrance e in _graphModel.EntrancesByLevel[1])
        {
            Cluster c1 = e.Cluster1;
            Cluster c2 = e.Cluster2;
            _edgeManager.AddHPAEdge(e.Node1, e.Node2, 1, 1, HPAEdgeType.INTER);
        }

        foreach (Cluster c in _graphModel.ClusterByLevel[1])
        {
            _clusterManager.CreateEdgesForCluster(c, c.bottomLeftPos.x, c.bottomLeftPos.y, c.topRightPos.x, c.topRightPos.y, 10);
            foreach (Entrance e1 in c.Entrances)
            {
                foreach (Entrance e2 in c.Entrances)
                {
                    if (e1.Node1 != e2.Node1)
                    {
                        double d = _pathFinder.CalculateDistance(_pathFinder.FindLocalPath(e1.Node1, e2.Node1, c)); //TODO: safe path in memory in special path class? 
                        if (d < double.PositiveInfinity)
                        {
                            _edgeManager.AddHPAEdge(e1.Node1, e2.Node1, d, 1, HPAEdgeType.INTER);
                        }
                    }
                }
            }
        }
    }


    public void AddLevelToGraph(int l)
    {
        if (!_graphModel.ClusterByLevel.ContainsKey(l - 1))
        {
            Debug.LogError("Previous level does not exist, cannot build level " + l);
            return;
        }

        if (!_graphModel.ClusterByLevel.ContainsKey(l))
        {
            _graphModel.ClusterByLevel.Add(l, new HashSet<Cluster>());
        }

        if (!_graphModel.EntrancesByLevel.ContainsKey(l))
        {
            _graphModel.EntrancesByLevel.Add(l, new HashSet<Entrance>());
        }


        var oldClusters = _graphModel.ClusterByLevel[l - 1].ToList();
        int numClusters = oldClusters.Count;
        int numClustersPerSide = (int)Math.Sqrt(numClusters);
        int clustersToMergePerSide = 2; // Adjust this value as needed for the desired cluster merging size

        // Calculate the number of complete blocks we can merge in both dimensions
        int fullBlocksPerSide = numClustersPerSide / clustersToMergePerSide;
        int remainder = numClustersPerSide % clustersToMergePerSide;

        // Merge full blocks
        for (int i = 0; i < fullBlocksPerSide * clustersToMergePerSide; i += clustersToMergePerSide)
        {
            for (int j = 0; j < fullBlocksPerSide * clustersToMergePerSide; j += clustersToMergePerSide)
            {
                List<Cluster> clustersToMerge = new List<Cluster>();
                for (int di = 0; di < clustersToMergePerSide; di++)
                {
                    for (int dj = 0; dj < clustersToMergePerSide; dj++)
                    {
                        int index = (i + di) * numClustersPerSide + (j + dj);
                        clustersToMerge.Add(oldClusters[index]);
                    }
                }
                MergeClusters(clustersToMerge, l);
            }
        }

        // Handle remainders for rows and columns

        // Remaining rows
        for (int i = fullBlocksPerSide * clustersToMergePerSide; i < numClustersPerSide; i++)
        {
            for (int j = 0; j < numClustersPerSide; j++)
            {
                Cluster oldCluster = oldClusters[i * numClustersPerSide + j];
                IncreaseSingleClusterLevel(oldCluster);
            }
        }

        // Remaining columns
        for (int j = fullBlocksPerSide * clustersToMergePerSide; j < numClustersPerSide; j++)
        {
            for (int i = 0; i < fullBlocksPerSide * clustersToMergePerSide; i++)
            {
                Cluster oldCluster = oldClusters[i * numClustersPerSide + j];
                IncreaseSingleClusterLevel(oldCluster);
            }

        }

        // Possible remaining square
        if (remainder > 0)
        {
            for (int i = fullBlocksPerSide * clustersToMergePerSide; i < numClustersPerSide; i++)
            {
                for (int j = fullBlocksPerSide * clustersToMergePerSide; j < numClustersPerSide; j++)
                {
                    Cluster oldCluster = oldClusters[i * numClustersPerSide + j];
                    IncreaseSingleClusterLevel(oldCluster);
                }
            }

        }

        Debug.Log("Added level " + l + " with " + _graphModel.ClusterByLevel[l].Count + " clusters.");


        foreach (Vector2Int nodePos in _graphModel.NodesByLevel[l].Keys)
        {
            HPANode n = _graphModel.NodesByLevel[l][nodePos];

            foreach (HPAEdge edge in _graphModel.NodesByLevel[l - 1][nodePos].Edges)
            {
                if (edge.Type == HPAEdgeType.INTER)
                {
                    _edgeManager.AddHPAEdge(n, _graphModel.NodesByLevel[l][edge.Node2.Position], edge.Weight, l, HPAEdgeType.INTRA);
                }
            }
        }

        foreach (Cluster cluster in _graphModel.ClusterByLevel[l])
        {
            Debug.Log("Cluster " + cluster.bottomLeftPos + " has " + cluster.Entrances.Count + " entrances at coordinates ");

            foreach (Entrance e1 in cluster.Entrances)
            {

                foreach (Entrance e2 in cluster.Entrances)
                {

                    if (e1 != e2) //e1.Node1.Cluster == e2.Node1.Cluster && e1.Node1.Cluster == cluster && e2.Node1.Cluster == cluster && e1.Node1 != e2.Node1)
                    {
                        double d = _pathFinder.CalculateDistance(_pathFinder.FindLocalPath(e1.Node1, e2.Node2, cluster)); //TODO: safe path in memory in special path class? 
                        if (d < double.PositiveInfinity)
                        {
                            _edgeManager.AddHPAEdge(e1.Node1, e2.Node2, 1, l, HPAEdgeType.INTER);
                        }
                    }
                }
            }
        }

        Debug.Log("Added level " + l + " with " + _graphModel.ClusterByLevel[l].Count + " clusters.");
    }

    private Cluster IncreaseSingleClusterLevel(Cluster c)
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

    private Cluster MergeClusters(List<Cluster> clustersToMerge, int level)
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



    private Cluster CreateNewClusterFromEntrances(int startX, int startY, int size, int level, HashSet<Cluster> oldClusters)
    {
        HashSet<HPANode> newNodes = new HashSet<HPANode>();
        HashSet<Entrance> newEntrances = new HashSet<Entrance>();

        Vector2Int bottomLeft = new Vector2Int(int.MaxValue, int.MaxValue);
        Vector2Int topRight = new Vector2Int(int.MinValue, int.MinValue);

        // Loop through each old cluster within the bounds of the new cluster
        for (int di = 0; di < size; di++)
        {
            for (int dj = 0; dj < size; dj++)
            {
                int index = (startX + di) * size + (startY + dj);
                Cluster oldCluster = oldClusters.ElementAt(index);

                // Update bounds
                bottomLeft = new Vector2Int(Math.Min(bottomLeft.x, oldCluster.bottomLeftPos.x), Math.Min(bottomLeft.y, oldCluster.bottomLeftPos.y));
                topRight = new Vector2Int(Math.Max(topRight.x, oldCluster.topRightPos.x), Math.Max(topRight.y, oldCluster.topRightPos.y));

                // Add nodes that are entrances and are within the new cluster bounds
                foreach (Entrance entrance in oldCluster.Entrances)
                {
                    if (entrance.Node1.Cluster == oldCluster && entrance.Node2.Cluster != oldCluster)
                    {
                        newNodes.Add(entrance.Node1);
                        newNodes.Add(entrance.Node2);
                    }
                    else if (entrance.Node1.Cluster != oldCluster && entrance.Node2.Cluster == oldCluster)
                    {
                        newNodes.Add(entrance.Node1);
                        newNodes.Add(entrance.Node2);
                    }
                }
            }
        }

        // Convert internal entrances to intra edges
        foreach (var node in newNodes)
        {
            foreach (var edge in node.Edges.ToList())
            {
                if (newNodes.Contains(edge.Node2))  // Both nodes of the edge are within the new cluster
                {
                    _edgeManager.AddHPAEdge(node, edge.Node2, edge.Weight, level, HPAEdgeType.INTRA);
                    node.Edges.Remove(edge);
                }
                else  // Edge crosses the boundary of the new cluster
                {
                    newEntrances.Add(new Entrance(node.Cluster, edge.Node2.Cluster, node, edge.Node2));
                }
            }
        }

        return new Cluster(level, newNodes, newEntrances, bottomLeft, topRight);
    }







    public List<HPANode> HierarchicalSearch(Vector2Int start, Vector2Int goal, int level)
    {

        _nodeManager.insertCheckpoint(start, level);
        _nodeManager.insertCheckpoint(goal, level);

        HPANode TempStart = _nodeManager.GetNodeByPosition(start, level);
        HPANode TempGoal = _nodeManager.GetNodeByPosition(goal, level);

        List<HPANode> abstractPath = _pathFinder.FindAbstractPath(TempStart, TempGoal, level);
        if (abstractPath == null)
        {
            return null;
        }

        List<HPANode> refinedPath = _pathFinder.RefinePath(abstractPath, level);
        // Optional: Smoothing the path if needed
        return refinedPath;
    }



    private bool Adjacent(Cluster c1, Cluster c2)
    {
        return c1.bottomLeftPos.x == c2.topRightPos.x + 1 || c1.topRightPos.x == c2.bottomLeftPos.x - 1 ||
               c1.bottomLeftPos.y == c2.topRightPos.y + 1 || c1.topRightPos.y == c2.bottomLeftPos.y - 1;
    }
}