

using System;

public class HPAEdge
{
    public Guid Id { get; set; }
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }
    public double Weight { get; set; }
    public int Level { get; set; }
    public HPAEdgeType Type { get; set; }

    public HPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type)
    {
        Id = Guid.NewGuid();
        Node1 = node1;
        Node2 = node2;
        Weight = weight;
        Level = level;
        Type = type;
    }
}
public enum HPAEdgeType { INTER, INTRA }