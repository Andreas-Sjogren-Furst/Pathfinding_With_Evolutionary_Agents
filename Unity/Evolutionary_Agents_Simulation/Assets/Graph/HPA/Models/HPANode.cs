using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class HPANode : IComparable<HPANode>
{



    public int Id { get; set; }
    public Cluster Cluster { get; set; }
    public int Level { get; set; }

    public int gCost; // Cost from start to node
    public int hCost; // Heuristic cost from node to end
    public int fCost => gCost + hCost; // Total cost
    public HPANode parent;

    public Vector2Int Position { get; set; }


    public int CompareTo(HPANode other)
    {
        if (other == null) return 1;

        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            // If fCost is the same, further compare by hCost to maintain a consistent ordering
            compare = hCost.CompareTo(other.hCost);
        }
        return compare;
    }



    public HashSet<HPAEdge> Edges { get; set; }
    public HPANode(int id, Cluster cluster, Vector2Int position, int level)
    {
        Id = id;
        Position = position;
        Cluster = cluster;
        Level = level;
        Edges = new HashSet<HPAEdge>();
    }


    public void Merge(HPANode other)
    {
        // Assuming Id, Position, Cluster, and Level are invariant and should not change during merge
        // Merge the edges from the other node into this one
        foreach (var edge in other.Edges)
        {
            if (!Edges.Any(e => e.Node2 == edge.Node2))
            {
                Edges.Add(edge);
            }
        }

    }

    public override bool Equals(object obj)
    {
        if (obj is HPANode other)
        {
            return this.Position == other.Position;
        }
        return false;
    }

    public override int GetHashCode()
    {
        // Create a hash code that is based on the position.
        // You might use a combination of x and y coordinates to do this.
        return Position.GetHashCode();
    }







}