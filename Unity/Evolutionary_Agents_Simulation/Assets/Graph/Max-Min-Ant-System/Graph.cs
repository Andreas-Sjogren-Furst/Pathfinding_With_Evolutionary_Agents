// written by: Gustav Clausen s214940
using System;
using System.Collections.Generic;
using System.Linq;

public class Graph
{
    public List<Node> Nodes { get; set; } // nodes. 
    // public double[,] AdjacencyMatrix { get; set; } // Edges.

    public Dictionary<int, double> Edges { get; set; } // Edges.



    public Graph()
    {
        Nodes = new List<Node>();
        Edges = new Dictionary<int, double>();
    }

    public void AddNode(Node node)
    {
        Nodes.Add(node);
    }

    public void RemoveNode(Node node)
    {
        Nodes.Remove(node);
    }

    public void AddEdge(Node source, Node destination, double distance)
    {
        int key = source.Id * 1000 + destination.Id;
        Edges[key] = distance;
    }

    public double getEdge(Node source, Node destination)
    {
        int key = source.Id * 1000 + destination.Id;
        if (Edges.TryGetValue(key, out double distance))
        {
            return distance;
        }
        return Double.MaxValue;
    }

    public void RemoveEdge(Node source, Node destination)
    {
        int key = source.Id * 1000 + destination.Id;
        Edges.Remove(key);
    }

    public void RemoveIsolatedNodes()
    {
        List<Node> isolatedNodes = new List<Node>();

        foreach (Node node in Nodes)
        {
            bool isConnected = false;
            foreach (Node otherNode in Nodes)
            {
                if (node != otherNode && (getEdge(node, otherNode) < Double.MaxValue || getEdge(otherNode, node) < Double.MaxValue))
                {
                    isConnected = true;
                    break;
                }
            }

            if (!isConnected)
            {
                isolatedNodes.Add(node);
            }
        }

        foreach (Node isolatedNode in isolatedNodes)
        {
            foreach (Node node2 in Nodes)
            {
                RemoveEdge(isolatedNode, node2);
                RemoveEdge(node2, isolatedNode);
            }
            RemoveNode(isolatedNode);

        }
    }



    public bool IsFullyConnected()
    {
        var visited = new HashSet<Node>();
        var toVisit = new Queue<Node>();

        if (Nodes.Count == 0) return true;

        toVisit.Enqueue(Nodes[0]);
        while (toVisit.Count > 0)
        {
            var node = toVisit.Dequeue();
            if (!visited.Contains(node))
            {
                visited.Add(node);
                foreach (var otherNode in Nodes)
                {
                    if (node != otherNode && (getEdge(node, otherNode) < Double.MaxValue || getEdge(otherNode, node) < Double.MaxValue))
                    {
                        toVisit.Enqueue(otherNode);
                    }
                }
            }
        }
        return visited.Count == Nodes.Count;
    }

    public static double ScaleValue(double originalValue, double originalMin, double originalMax, double newMin, double newMax)
    {
        if (originalMin == originalMax)
        {
            throw new ArgumentException("The min and max values must be different.");
        }

        return ((newMax - newMin) * (originalValue - originalMin) / (originalMax - originalMin)) + newMin;
    }


    public void ScaleGraphEdges(double newMin, double newMax)
    {
        double originalMin = double.MaxValue;
        double originalMax = double.MinValue;

        // Find the original min and max edge values
        foreach (var edge in Edges)
        {
            if (edge.Value < originalMin)
            {
                originalMin = edge.Value;
            }
            if (edge.Value > originalMax)
            {
                originalMax = edge.Value;
            }
        }

        // Create a list of keys to iterate over
        List<int> keys = Edges.Keys.ToList();

        // Scale the edge values
        foreach (var key in keys)
        {
            Edges[key] = ScaleValue(Edges[key], originalMin, originalMax, newMin, newMax);
        }
    }


}