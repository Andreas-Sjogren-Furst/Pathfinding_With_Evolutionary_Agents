// written by: Gustav Clausen s214940

public class HPAEdge
{
    public HPANode Node1 { get; set; }
    public HPANode Node2 { get; set; }
    public double Weight { get; set; }
    public int Level { get; set; }
    public HPAEdgeType Type { get; set; }
    public HPAPath IntraPaths { get; set; }

    public HPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type)
    {
        Node1 = node1;
        Node2 = node2;
        Weight = weight;
        Level = level;
        Type = type;
    }

    public HPAEdge(HPANode node1, HPANode node2, double weight, int level, HPAEdgeType type, HPAPath IntraPath)
    {
        Node1 = node1;
        Node2 = node2;
        Weight = weight;
        Level = level;
        Type = type;
        IntraPaths = IntraPath;
    }

    public override bool Equals(object obj)
    {
        if (obj is HPAEdge other)
        {
            return this.Node1.Equals(other.Node1) && this.Node2.Equals(other.Node2);
        }
        return false;
    }

    public override int GetHashCode()
    {
        // Create a hash code that is based on the position.
        // You might use a combination of x and y coordinates to do this.
        return Node1.GetHashCode() + Node2.GetHashCode() + Level.GetHashCode() + Type.GetHashCode();
    }

}

// public class HPAInterEdge : HPAEdge
// {
//     public HPAPath IntraPaths { get; set; }

//     public HPAInterEdge(HPANode node1, HPANode node2, double weight, int level, HPAPath IntraPath)
//         : base(node1, node2, weight, level, HPAEdgeType.INTER)
//     {
//         IntraPaths = IntraPath;
//     }

//     // You can override methods or add new ones specific to inter-cluster edges
// }
public enum HPAEdgeType { INTER, INTRA }