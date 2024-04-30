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

    public HPAEdge[] UpdateEdgesFromRemovedNode(HPANode n)
    {
        // Convert edges to an array or list to safely modify the collection during iteration
        HPAEdge[] edges = n.Edges.ToArray();

        foreach (HPAEdge edge in edges)
        {
            // Remove the edge from the node being deleted
            if (edge.Node1 != null)
            {
                edge.Node1.Edges.Remove(edge);
                // Also remove the reciprocal edge from Node2 if it's not null
                var reciprocal = edge.Node2.Edges.FirstOrDefault(e => e.Node2 == edge.Node1);
                if (reciprocal != null)
                {
                    edge.Node2.Edges.Remove(reciprocal);
                }
            }
            if (edge.Node2 != null)
            {
                edge.Node2.Edges.Remove(edge);
                // Similarly, remove the reciprocal edge from Node1
                var reciprocal = edge.Node1.Edges.FirstOrDefault(e => e.Node2 == edge.Node2);
                if (reciprocal != null)
                {
                    edge.Node1.Edges.Remove(reciprocal);
                }
            }
        }
        return edges;
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