using System.Buffers.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

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



    public List<HPAEdge> Edges { get; set; }
    public HPANode(int id, Cluster cluster, Vector2Int position, int level)
    {
        Id = id;
        Position = position;
        Cluster = cluster;
        Level = level;
        Edges = new List<HPAEdge>();
    }

    // cast 






}