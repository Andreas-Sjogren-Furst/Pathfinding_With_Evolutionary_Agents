using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EdgeManager : IEdgeManager
{

    private readonly IPathFinder _pathFinder;

    public EdgeManager(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }
    public void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type)
    {
        HPAEdge edge1 = new HPAEdge(node1, node2, weight, level, type);
        HPAEdge edge2 = new HPAEdge(node2, node1, weight, level, type);
        node1.Edges.Add(edge1);
        node2.Edges.Add(edge2);
    }

    public void ConnectToBorder(HPANode n, Cluster c)
    {
        int level = c.Level;
        HashSet<HPANode> borderNodes = new HashSet<HPANode>();
        foreach (Entrance e in c.Entrances)
        {
            if (e.Node1 != n) borderNodes.Add(e.Node1);
            if (e.Node2 != n && e.Node2 != null) borderNodes.Add(e.Node2);
        }

        foreach (HPANode borderNode in borderNodes)
        {
            double distance = _pathFinder.CalculateDistance(_pathFinder.FindLocalPath(n, borderNode, c));
            if (distance < double.PositiveInfinity)
            {
                AddHPAEdge(n, borderNode, distance, level, HPAEdgeType.INTER);
            }
        }
    }
}