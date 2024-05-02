using System;
using System.Linq;

public class Entrance
{
    // public Guid Id { get; set; }
    public Cluster Cluster1 { get; set; }
    public Cluster Cluster2 { get; set; }
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }

    public HPAEdge Edge1 { get { return Node1.Edges.First(e => e.Node2 == Node2); } }
    public HPAEdge Edge2 { get { return Node2.Edges.First(e => e.Node2 == Node1); } }

    public Entrance(Cluster cluster1, Cluster cluster2, HPANode node1, HPANode node2)
    {
        // Id = Guid.NewGuid();
        Cluster1 = cluster1;
        Cluster2 = cluster2;
        Node1 = node1;
        Node2 = node2;
    }

    public override bool Equals(object obj)
    {
        if (obj is Entrance other)
        {
            // Check if the entrances connect the same nodes in the same order
            return Node1.Equals(other.Node1.Position) && Node2.Equals(other.Node2.Position);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Node1.GetHashCode();

    }
}