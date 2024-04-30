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
            _edgeManager.AddHPAEdge(e.Node1, e.Node2, 1, 1, HPAEdgeType.INTER); // adding inter edges between nodes that consists an entrance (2 connected nodes).
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
                _clusterManager.MergeClusters(clustersToMerge, l);
            }
        }

        // Handle remainders for rows and columns

        // Remaining rows
        for (int i = fullBlocksPerSide * clustersToMergePerSide; i < numClustersPerSide; i++)
        {
            for (int j = 0; j < numClustersPerSide; j++)
            {
                Cluster oldCluster = oldClusters[i * numClustersPerSide + j];
                _clusterManager.IncreaseSingleClusterLevel(oldCluster);
            }
        }

        // Remaining columns
        for (int j = fullBlocksPerSide * clustersToMergePerSide; j < numClustersPerSide; j++)
        {
            for (int i = 0; i < fullBlocksPerSide * clustersToMergePerSide; i++)
            {
                Cluster oldCluster = oldClusters[i * numClustersPerSide + j];
                _clusterManager.IncreaseSingleClusterLevel(oldCluster);
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
                    _clusterManager.IncreaseSingleClusterLevel(oldCluster);
                }
            }

        }

        Debug.Log("Added level " + l + " with " + _graphModel.ClusterByLevel[l].Count + " clusters.");


        // convert previous inter edges to intra 
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
            // Debug.Log("Cluster " + cluster.bottomLeftPos + " has " + cluster.Entrances.Count + " entrances at coordinates ");

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

    public void insertNodeGlobally(Vector2Int pos, int maxLevel)
    {

        // iterate through levels
        for (int i = 1; i <= maxLevel; i++)
        {

            // Implement algorithm to update the graph dynamically across all levels. 


        }
    }


    public void DynamicallyAddHPANode(Vector2Int position, Boolean isFinalNodeInCluster = false)
    {
        Cluster cluster = _clusterManager.DetermineCluster(position, 1);
        Debug.Log("found cluster " + cluster.bottomLeftPos + " " + cluster.topRightPos);
        cluster.isFinalized = isFinalNodeInCluster;
        HPANode discoveredNode = _nodeManager.FindOrCreateNode(position.x, position.y, cluster);
        cluster.Nodes.Add(discoveredNode);

        updateRelevantEdges(cluster, position);
        if (!cluster.isFinalized)
        {
            // Debug.Log("Adding node at " + position);

            _clusterManager.CreateEdgesForCluster(cluster, cluster.bottomLeftPos.x, cluster.bottomLeftPos.y, cluster.topRightPos.x, cluster.topRightPos.y, 10);

            // build the adjenct clusters
            foreach (Cluster cNeighbor in _graphModel.ClusterByLevel[1])
            {
                if (Adjacent(cluster, cNeighbor))
                {
                    HashSet<Entrance> entrances = _entranceManager.BuildEntrances(cluster, cNeighbor);
                    cluster.Entrances.UnionWith(entrances);
                    cNeighbor.Entrances.UnionWith(entrances);
                    if (!_graphModel.EntrancesByLevel.ContainsKey(1))
                    {
                        _graphModel.EntrancesByLevel.Add(1, new HashSet<Entrance>());
                    }
                    _graphModel.EntrancesByLevel[1].UnionWith(entrances);
                }
            }
        }

    }

    private void updateRelevantEdges(Cluster cluster, Vector2Int newNodePosition)
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
                Cluster adjacentCluster = _clusterManager.DetermineCluster(adjacentPosition, 1);

                if (adjacentCluster.isOnBorder(adjacentPosition))
                { // if the other node is on the border, we create a inter edge TOOD: perhaps we should create entrance instead. 
                    _edgeManager.AddHPAEdge(_graphModel.NodesByLevel[1][newNodePosition], _graphModel.NodesByLevel[1][adjacentPosition], 1, 1, HPAEdgeType.INTER);
                }



            }
        }
    }




    private bool Adjacent(Cluster c1, Cluster c2)
    {
        return c1.bottomLeftPos.x == c2.topRightPos.x + 1 || c1.topRightPos.x == c2.bottomLeftPos.x - 1 ||
               c1.bottomLeftPos.y == c2.topRightPos.y + 1 || c1.topRightPos.y == c2.bottomLeftPos.y - 1;
    }


}