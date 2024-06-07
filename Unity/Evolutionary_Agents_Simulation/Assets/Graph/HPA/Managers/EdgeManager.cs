// written by: Gustav Clausen s214940
using System.Collections.Generic;
using System.Linq;

public class EdgeManager : IEdgeManager
{

    private readonly IPathFinder _pathFinder;

    public EdgeManager(IPathFinder pathFinder)
    {
        _pathFinder = pathFinder;
    }
    public void AddHPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type, HPAPath IntraPath = null)
    {

        if (IntraPath != null)
        {
            // if (IntraPath.Length == 0)
            // {
            //     foreach (HPANode n in IntraPath.path)
            //     {
            //         Debug.Log(n.Position);
            //     }
            //     throw new ArgumentException("IntraPath length must be greater than 0");
            // }
            HPAEdge edge3 = new HPAEdge(node1, node2, weight, level, type, IntraPath);
            HPAEdge edge4 = new HPAEdge(node2, node1, weight, level, type, IntraPath);
            node1.Edges.Add(edge3);
            node2.Edges.Add(edge4);
            return;


        }


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


    public List<HPAPath> ConnectToBorder(HPANode n, Cluster c)
    {
        int level = c.Level;
        HashSet<HPANode> borderNodes = new HashSet<HPANode>();
        List<HPAPath> paths = new List<HPAPath>();
        foreach (Entrance e in c.Entrances)
        {
            if (e.Node1 != n && c.Contains(e.Node1.Position)) borderNodes.Add(e.Node1);
            if (e.Node2 != n && c.Contains(e.Node2.Position)) borderNodes.Add(e.Node2);
        }

        foreach (HPANode borderNode in borderNodes)
        {
            if (borderNode.Level != level)
            {
                // Debug.LogError("Border node level does not match cluster level");
                continue;
            }

            if (!c.Contains(borderNode.Position))
            {
                // Debug.LogError("Border node does not belong to the specified cluster at " + borderNode.Position + " and " + c.bottomLeftPos + " and " + c.topRightPos);
                continue;
            }
            HPAPath path = _pathFinder.FindLocalPath(n, borderNode, c);
            double distance = path?.Length ?? double.PositiveInfinity;



            if (distance < double.PositiveInfinity) //TODO: den kan ikke finde nogle paths... på levels højere 1
            {
                // Debug.Log($"Connecting {n.Position} to {borderNode.Position} at level {level} with distance {distance}");
                AddHPAEdge(n, borderNode, distance, level, HPAEdgeType.INTER, IntraPath: path);
                paths.Add(path);


            }
            else
            {
                // Debug.Log($"No path found between {n.Position} and {borderNode.Position} at level {level} in cluster {c.bottomLeftPos} and {c.topRightPos}");
                // Debug.Log($"Cluster positions {c.bottomLeftPos} and {c.topRightPos} and {borderNode.Cluster.bottomLeftPos} and {n.Cluster.bottomLeftPos}");
                // Debug.Log($"path length: {distance} and path: {path.path.Count}");

            }
        }
        return paths;
    }
}