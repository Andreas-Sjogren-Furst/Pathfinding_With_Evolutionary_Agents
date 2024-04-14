using System.Collections.Generic;
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
            foreach (Entrance e1 in c.Entrances)
            {
                foreach (Entrance e2 in c.Entrances)
                {
                    if (e1.Id != e2.Id)
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
        _graphModel.ClusterByLevel[l] = _clusterManager.BuildClusters(l, _graphModel.GlobalTileMap);
        foreach (Cluster c1 in _graphModel.ClusterByLevel[l])
        {
            foreach (Cluster c2 in _graphModel.ClusterByLevel[l])
            {
                if (!Adjacent(c1, c2))
                    continue;

                foreach (Entrance e in _entranceManager.GetEntrances(c1, c2))
                {
                    e.Node1.Level = l;
                    e.Node2.Level = l;



                    // SetLevel(GetHPAEdge(e), l);
                }
            }
        }

        foreach (Cluster c in _graphModel.ClusterByLevel[l])
        {
            foreach (HPANode n1 in c.Nodes)
            {
                foreach (HPANode n2 in c.Nodes)
                {
                    if (n1 != n2)
                    {
                        double d = _pathFinder.CalculateDistance(_pathFinder.FindLocalPath(n1, n2, c)); //TODO: safe path in memory in special path class? 
                        if (d < double.PositiveInfinity)
                        {
                            _edgeManager.AddHPAEdge(n1, n2, d, l, HPAEdgeType.INTRA);
                        }
                    }
                }
            }
        }
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